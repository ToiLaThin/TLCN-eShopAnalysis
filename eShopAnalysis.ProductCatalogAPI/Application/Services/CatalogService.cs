using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract;
using System.Runtime.CompilerServices;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface ICatalogService
    {
        ServiceResponseDto<Catalog> Get(Guid catalogGuid);
        ServiceResponseDto<IEnumerable<Catalog>> GetAll();
        ServiceResponseDto<Catalog> AddCatalog(Catalog catalog);
        bool DeleteCatalog(Guid catalogId);
        ServiceResponseDto<Catalog> UpdateCatalog(Catalog catalog); //for testing only
        ServiceResponseDto<Catalog> UpdateCatalogInfo(Catalog catalog);//only modify name and info

        //SubCatalog manipulate from catalog
        ServiceResponseDto<SubCatalog> GetSubCatalog(Guid catalogId, Guid subCatalogId);
        ServiceResponseDto<IEnumerable<SubCatalog>> GetAllSubCatalogs(Guid catalogId);
        bool AddNewSubCatalog(Guid catalogId, SubCatalog subCatalog);
        ServiceResponseDto<SubCatalog> UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog );
        ServiceResponseDto<SubCatalog> DeleteSubCatalog(Guid catalogId,Guid subCatalogId);
    }

    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CatalogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ServiceResponseDto<IEnumerable<Catalog>> GetAll()
        {
            var result = _unitOfWork.CatalogRepository.GetAll();
            return ServiceResponseDto<IEnumerable<Catalog>>.Success(result);
        }

        public ServiceResponseDto<Catalog> Get(Guid catalogGuid)
        {
            var result = _unitOfWork.CatalogRepository.Get(catalogGuid);
            if (result == null)
            {
                return ServiceResponseDto<Catalog>.Failure("cannot find catalog");
            }
            return ServiceResponseDto<Catalog>.Success(result);
        }

        public ServiceResponseDto<Catalog> AddCatalog(Catalog catalog)
        {
            var result = _unitOfWork.CatalogRepository.Add(catalog);
            return ServiceResponseDto<Catalog>.Success(result);
        }

        public ServiceResponseDto<Catalog> UpdateCatalog(Catalog catalog)
        {
            bool success = _unitOfWork.CatalogRepository.Update(catalog);
            if (success == true) {
                return Get(catalog.CatalogId);
            }
            return ServiceResponseDto<Catalog>.Failure("Update catalog failed");
        }

        public ServiceResponseDto<Catalog> UpdateCatalogInfo(Catalog catalog)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCatalog(Guid catalogId)
        {
            bool success = _unitOfWork.CatalogRepository.Remove(catalogId);
            if (success == true) {
                return true;
            }
            return false;
        }

        public ServiceResponseDto<SubCatalog> GetSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            Catalog catalogToGet = _unitOfWork.CatalogRepository.Find(c => c.CatalogId == catalogId).First();
            if (catalogToGet is null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find catalog to update subcatalog");
            }
            SubCatalog subCatalog = catalogToGet.SubCatalogs.Where(sc => sc.SubCatalogId == subCatalogId).FirstOrDefault();
            if (subCatalog is null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find subcatalog to update");
            }
            return ServiceResponseDto<SubCatalog>.Success(subCatalog);
        }

        public ServiceResponseDto<IEnumerable<SubCatalog>> GetAllSubCatalogs(Guid catalogId)
        {
            Catalog catalogToGet = _unitOfWork.CatalogRepository.Find(c => c.CatalogId == catalogId).First();
            if (catalogToGet is null) {
                return ServiceResponseDto<IEnumerable<SubCatalog>>.Failure("cannot find catalog to get all subcatalog");
            }
            return ServiceResponseDto<IEnumerable<SubCatalog>>.Success(catalogToGet.SubCatalogs);
        }

        public bool AddNewSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _unitOfWork.CatalogRepository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return false;
            }
            catalogToAddSubTo.AddSubCatalog(subCatalog);
            var result = _unitOfWork.CatalogRepository.Update(catalogToAddSubTo);
            return result;
        }
        
        public ServiceResponseDto<SubCatalog> UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _unitOfWork.CatalogRepository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find catalog to update subcatalog");
            }
            SubCatalog updateSubCatalog = catalogToAddSubTo.UpdateExistingSubCatalog(subCatalog); //the domain driven design help simplify the logic here, we do not have to find then check then remove then update
            if (updateSubCatalog == null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find subcatalog to update");
            }
            var result = _unitOfWork.CatalogRepository.Update(catalogToAddSubTo);
            if (result == false) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot update subcatalog");
            }
            return ServiceResponseDto<SubCatalog>.Success(updateSubCatalog);
        }

        public ServiceResponseDto<SubCatalog> DeleteSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _unitOfWork.CatalogRepository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot find catalog to delete subcatalog");
            }
            SubCatalog deleteCatalog = catalogToAddSubTo.RemoveExistingSubCatalog(subCatalogId);
            if (deleteCatalog is null) { 
                return ServiceResponseDto<SubCatalog>.Failure("cannot remove subcatalog from catalog in model"); 
            }

            var result = _unitOfWork.CatalogRepository.Update(catalogToAddSubTo);
            if (result == false) {
                return ServiceResponseDto<SubCatalog>.Failure("cannot remove subcatalog from catalog");
            }
            return ServiceResponseDto<SubCatalog>.Success(deleteCatalog);

        }
    }
}
