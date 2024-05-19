using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;
using eShopAnalysis.StockProviderRequestAPI.Repository;
using System.Collections.Generic;

namespace eShopAnalysis.StockProviderRequestAPI.Service
{
    public class ProviderRequirementService : IProviderRequirementService
    {
        private readonly IProviderRequirementRepository _providerReqRepo;

        public ProviderRequirementService(IProviderRequirementRepository providerReqRepo)
        {
            _providerReqRepo = providerReqRepo;
        }

        public async Task<ServiceResponseDto<ProviderRequirement>> Add(ProviderRequirement providerReqToAdd)
        {
            ProviderRequirement providerRequirementToFind = await _providerReqRepo.GetByNameAsync(providerReqToAdd.ProviderName);
            if (providerRequirementToFind != null) {
                return ServiceResponseDto<ProviderRequirement>.Failure("cannot add provider requirement  because provider with same name already exist");
            }

            var providerReqAdded = await _providerReqRepo.AddAsync(providerReqToAdd);
            if (providerReqAdded == null) {
                return ServiceResponseDto<ProviderRequirement>.Failure("Cannot added provider requirement because cannot find it, please check repo result");
            }

            return ServiceResponseDto<ProviderRequirement>.Success(providerReqAdded);
        }

        //not implementing the logic to check if all providerReqs not exists
        public async Task<ServiceResponseDto<string>> AddRange(IEnumerable<ProviderRequirement> providerReqsToAdd)
        {
            await _providerReqRepo.AddRangeAsync(providerReqsToAdd);
            return ServiceResponseDto<string>.Success("add all provider reqs successfully");

        }

        public async Task<ServiceResponseDto<IEnumerable<ProviderRequirement>>> GetAll()
        {
            var allProviderReqs = _providerReqRepo.GetAllAsQueryable().ToList();
            if (allProviderReqs == null) {
                return ServiceResponseDto<IEnumerable<ProviderRequirement>>.Failure("result is null");
            }
            return ServiceResponseDto<IEnumerable<ProviderRequirement>>.Success(allProviderReqs);
        }

        public async Task<ServiceResponseDto<ProviderRequirement>> GetByName(string name)
        {
            ProviderRequirement providerReqToFind = await _providerReqRepo.GetByNameAsync(name);
            if (providerReqToFind == null) {
                return ServiceResponseDto<ProviderRequirement>.Failure("Cannot find provider requirement of that name");
            }
            return ServiceResponseDto<ProviderRequirement>.Success(providerReqToFind);
        }

        public async Task<ServiceResponseDto<IEnumerable<StockItemRequestMeta>>> GetStockItemRequestMetasWithProductModelIds(IEnumerable<Guid> productModelIds)
        {
            var productModelIdsDistinct = productModelIds.Distinct();
            var stockItemRequestMetasWithProductModelIds = _providerReqRepo.GetAllAsQueryable()
                .Where(
                    p => p.AvailableStockItemRequestMetas.Any(
                        stockItemReqMeta => productModelIdsDistinct.Contains(stockItemReqMeta.ProductModelId)
                    )
                )
                .SelectMany(p => p.AvailableStockItemRequestMetas.Where(
                    stockItemReqMeta => productModelIdsDistinct.Contains(stockItemReqMeta.ProductModelId))
                )
                .ToList();
            if (stockItemRequestMetasWithProductModelIds == null || stockItemRequestMetasWithProductModelIds.Count <= 0) {
                return ServiceResponseDto<IEnumerable<StockItemRequestMeta>>.Failure("null stockItemRequest with those productModelIds");
            }
            return ServiceResponseDto<IEnumerable<StockItemRequestMeta>>.Success(stockItemRequestMetasWithProductModelIds);

        }

        public async Task<ServiceResponseDto<string>> Truncate()
        {
            var isSuccess = await _providerReqRepo.DeleteAllAsync();
            if (!isSuccess) {
                return ServiceResponseDto<string>.Failure("failed to truncate all document");
            }
            return ServiceResponseDto<string>.Success("truncated all document");
        }
    }
}
