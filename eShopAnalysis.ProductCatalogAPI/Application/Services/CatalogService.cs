using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract;
using System.Runtime.CompilerServices;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface ICatalogService
    {
        Catalog Get(Guid catalogGuid);
        ServiceResponseDto<IEnumerable<Catalog>> GetAll();
        Catalog AddCatalog(Catalog catalog);
        bool DeleteCatalog(Guid catalogId);
        Catalog UpdateCatalog(Catalog catalog); //for testing only
        Catalog UpdateCatalogInfo(Catalog catalog);//only modify name and info

        //SubCatalog manipulate from catalog
        SubCatalog GetSubCatalog(Guid catalogId, Guid subCatalogId);
        IEnumerable<SubCatalog> GetAllSubCatalogs(Guid catalogId);
        bool AddNewSubCatalog(Guid catalogId, SubCatalog subCatalog);
        SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog );
        SubCatalog DeleteSubCatalog(Guid catalogId,Guid subCatalogId);
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

        public Catalog Get(Guid catalogGuid)
        {
            var result = _unitOfWork.CatalogRepository.Get(catalogGuid);
            return result;
        }

        public Catalog AddCatalog(Catalog catalog)
        {
            var result = _unitOfWork.CatalogRepository.Add(catalog);
            return result;
        }

        public Catalog UpdateCatalog(Catalog catalog)
        {
            bool success = _unitOfWork.CatalogRepository.Update(catalog);
            if (success == true) {
                return Get(catalog.CatalogId);
            }
            return null;
        }

        public Catalog UpdateCatalogInfo(Catalog catalog)
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

        public SubCatalog GetSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            Catalog catalogToGet = _unitOfWork.CatalogRepository.Find(c => c.CatalogId == catalogId).First();
            if (catalogToGet is null) {
                return null;
            }
            SubCatalog subCatalog = catalogToGet.SubCatalogs.Where(sc => sc.SubCatalogId == subCatalogId).FirstOrDefault();
            if (subCatalog is null) { 
                return null;
            }
            return subCatalog;
        }

        public IEnumerable<SubCatalog> GetAllSubCatalogs(Guid catalogId)
        {
            Catalog catalogToGet = _unitOfWork.CatalogRepository.Find(c => c.CatalogId == catalogId).First();
            if (catalogToGet is null) {
                return null;
            }
            return catalogToGet.SubCatalogs;
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
        
        public SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _unitOfWork.CatalogRepository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return null;
            }
            else {
                SubCatalog updateSubCatalog = catalogToAddSubTo.UpdateExistingSubCatalog(subCatalog); //the domain driven design help simplify the logic here, we do not have to find then check then remove then update
                if (updateSubCatalog != null) {
                    var result = _unitOfWork.CatalogRepository.Update(catalogToAddSubTo);
                    if (result == true) {
                        return updateSubCatalog;
                    }
                }
                return null;
            }
        }

        public SubCatalog DeleteSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _unitOfWork.CatalogRepository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return null;
            }
            SubCatalog deleteCatalog = catalogToAddSubTo.RemoveExistingSubCatalog(subCatalogId);
            if (deleteCatalog is null) { return null; }

            var result = _unitOfWork.CatalogRepository.Update(catalogToAddSubTo);
            if (result == false) { return null; }
            return deleteCatalog;

        }
    }
}
