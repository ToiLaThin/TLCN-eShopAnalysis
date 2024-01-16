using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Service
{
    public interface IProviderRequirementService
    {
        Task<ServiceResponseDto<ProviderRequirement>> GetByName(string name);

        Task<ServiceResponseDto<IEnumerable<ProviderRequirement>>> GetAll();

        Task<ServiceResponseDto<ProviderRequirement>> Add(ProviderRequirement providerReqToAdd);

        Task<ServiceResponseDto<string>> AddRange(IEnumerable<ProviderRequirement> providerReqsToAdd);

        Task<ServiceResponseDto<string>> Truncate();
    }
}
