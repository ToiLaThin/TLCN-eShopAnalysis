using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract;
using System.Runtime.CompilerServices;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface ICatalogService
    {
        Task<ServiceResponseDto<Catalog>> Get(Guid catalogGuid);
        Task<ServiceResponseDto<IEnumerable<Catalog>>> GetAll();
        Task<ServiceResponseDto<Catalog>> AddCatalog(Catalog catalog);
        Task<bool> DeleteCatalog(Guid catalogId);
        Task<ServiceResponseDto<Catalog>> UpdateCatalog(Catalog catalog); //for testing only
        Task<ServiceResponseDto<Catalog>> UpdateCatalogInfo(Catalog catalog);//only modify name and info

        //SubCatalog manipulate from catalog
        Task<ServiceResponseDto<SubCatalog>> GetSubCatalog(Guid catalogId, Guid subCatalogId);
        Task<ServiceResponseDto<IEnumerable<SubCatalog>>> GetAllSubCatalogs(Guid catalogId);
        Task<bool> AddNewSubCatalog(Guid catalogId, SubCatalog subCatalog);
        Task<ServiceResponseDto<SubCatalog>> UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog );
        Task<ServiceResponseDto<SubCatalog>> DeleteSubCatalog(Guid catalogId,Guid subCatalogId);
    }

    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponseDto<IEnumerable<Catalog>>> GetAll()
        {
            var result = _unitOfWork.CatalogRepository.GetAllAsQueryable().ToList();
            return ServiceResponseDto<IEnumerable<Catalog>>.Success(result);
        }

        public async Task<ServiceResponseDto<Catalog>> Get(Guid catalogGuid)
        {
            var result = await _unitOfWork.CatalogRepository.GetAsync(catalogGuid);
            if (result == null)
            {
                return ServiceResponseDto<Catalog>.Failure("cannot find catalog");
            }
            return ServiceResponseDto<Catalog>.Success(result);
        }

        public async Task<ServiceResponseDto<Catalog>> AddCatalog(Catalog catalog)
        {
            var result = await _unitOfWork.CatalogRepository.AddAsync(catalog);
            return ServiceResponseDto<Catalog>.Success(result);
        }

        public async Task<ServiceResponseDto<Catalog>> UpdateCatalog(Catalog catalog)
        {
            bool success = await _unitOfWork.CatalogRepository.UpdateAsync(catalog);
            if (success == true) {
                return await Get(catalog.CatalogId);
            }
            return ServiceResponseDto<Catalog>.Failure("Update catalog failed");
        }

        public async Task<ServiceResponseDto<Catalog>> UpdateCatalogInfo(Catalog catalog)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteCatalog(Guid catalogId)
        {
            bool success = await _unitOfWork.CatalogRepository.RemoveAsync(catalogId);
            if (success == true) {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponseDto<SubCatalog>> GetSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            var catalogToGet = await _unitOfWork.CatalogRepository.GetAsync(catalogId);
            if (catalogToGet is null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find catalog to update subcatalog");
            }
            SubCatalog subCatalog = catalogToGet.SubCatalogs.Where(sc => sc.SubCatalogId == subCatalogId).FirstOrDefault();
            if (subCatalog is null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find subcatalog to update");
            }
            return ServiceResponseDto<SubCatalog>.Success(subCatalog);
        }

        public async Task<ServiceResponseDto<IEnumerable<SubCatalog>>> GetAllSubCatalogs(Guid catalogId)
        {
            var catalogToGet = await _unitOfWork.CatalogRepository.GetAsync(catalogId);
            if (catalogToGet is null) {
                return ServiceResponseDto<IEnumerable<SubCatalog>>.Failure("cannot find catalog to get all subcatalog");
            }
            return ServiceResponseDto<IEnumerable<SubCatalog>>.Success(catalogToGet.SubCatalogs);
        }

        public async Task<bool> AddNewSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = await _unitOfWork.CatalogRepository.GetAsync(catalogId);
            if (catalogToAddSubTo == null) {
                return false;
            }
            catalogToAddSubTo.AddSubCatalog(subCatalog);
            var result = await _unitOfWork.CatalogRepository.UpdateAsync(catalogToAddSubTo);
            return result;
        }
        
        public async Task<ServiceResponseDto<SubCatalog>> UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = await _unitOfWork.CatalogRepository.GetAsync(catalogId);
            if (catalogToAddSubTo == null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find catalog to update subcatalog");
            }
            SubCatalog updateSubCatalog = catalogToAddSubTo.UpdateExistingSubCatalog(subCatalog); //the domain driven design help simplify the logic here, we do not have to find then check then remove then update
            if (updateSubCatalog == null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find subcatalog to update");
            }
            var result = await _unitOfWork.CatalogRepository.UpdateAsync(catalogToAddSubTo);
            if (result == false) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot update subcatalog");
            }
            return ServiceResponseDto<SubCatalog>.Success(updateSubCatalog);
        }

        public async Task<ServiceResponseDto<SubCatalog>> DeleteSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = await _unitOfWork.CatalogRepository.GetAsync(catalogId);
            if (catalogToAddSubTo == null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find catalog to delete subcatalog");
            }
            SubCatalog deleteCatalog = catalogToAddSubTo.RemoveExistingSubCatalog(subCatalogId);
            if (deleteCatalog is null) { 
                return ServiceResponseDto<SubCatalog>.Failure("cannot remove subcatalog from catalog in model"); 
            }

            var result = await _unitOfWork.CatalogRepository.UpdateAsync(catalogToAddSubTo);
            if (result == false) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot remove subcatalog from catalog");
            }
            return ServiceResponseDto<SubCatalog>.Success(deleteCatalog);

        }
    }
}
