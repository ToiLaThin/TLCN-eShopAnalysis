using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using System.Runtime.CompilerServices;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface ICatalogService
    {
        Catalog Get(Guid catalogGuid);
        IEnumerable<Catalog> GetAll();
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
        private readonly ICatalogRepository _repository;
        public CatalogService(ICatalogRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Catalog> GetAll()
        {
            var result = _repository.GetAll();
            return result;
        }

        public Catalog Get(Guid catalogGuid)
        {
            var result = _repository.Get(catalogGuid);
            return result;
        }

        public Catalog AddCatalog(Catalog catalog)
        {
            var result = _repository.Add(catalog);
            return result;
        }

        public Catalog UpdateCatalog(Catalog catalog)
        {
            bool success = _repository.Update(catalog);
            if (success == true)
            {
                return Get(catalog.CatalogId);
            }
            else
            {
                return null;
            }
        }

        public Catalog UpdateCatalogInfo(Catalog catalog)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCatalog(Guid catalogId)
        {
            bool success = _repository.Remove(catalogId);
            if (success == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public SubCatalog GetSubCatalog(Guid catalogId, Guid subCatalogId)
        {
            Catalog catalogToGet = _repository.Find(c => c.CatalogId == catalogId).First();
            if (catalogToGet is null) {
                return null;
            }
            else
            {
                SubCatalog subCatalog = catalogToGet.SubCatalogs.Where(sc => sc.SubCatalogId == subCatalogId).FirstOrDefault();
                if (subCatalog is null)
                {
                    return null;
                }
                return subCatalog;
            }
        }

        public IEnumerable<SubCatalog> GetAllSubCatalogs(Guid catalogId)
        {
            Catalog catalogToGet = _repository.Find(c => c.CatalogId == catalogId).First();
            if (catalogToGet is null) {
                return null;
            }
            else
            {
                return catalogToGet.SubCatalogs;
            }
        }

        public bool AddNewSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _repository.Get(catalogId);
            if (catalogToAddSubTo == null)
            {
                return false;
            }
            else
            {
                catalogToAddSubTo.AddSubCatalog(subCatalog);
                var result = _repository.Update(catalogToAddSubTo);
                return result;
            }
        }
        
        public SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog)
        {
            //check if catalog exist: now how to remove this duplication
            var catalogToAddSubTo = _repository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return null;
            }
            else {
                SubCatalog updateSubCatalog = catalogToAddSubTo.UpdateExistingSubCatalog(subCatalog); //the domain driven design help simplify the logic here, we do not have to find then check then remove then update
                if (updateSubCatalog != null) {
                    var result = _repository.Update(catalogToAddSubTo);
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
            var catalogToAddSubTo = _repository.Get(catalogId);
            if (catalogToAddSubTo == null) {
                return null;
            }
            else {
                SubCatalog deleteCatalog = catalogToAddSubTo.RemoveExistingSubCatalog(subCatalogId);
                if (deleteCatalog is not null) {
                    var result = _repository.Update(catalogToAddSubTo);
                    if (result == true) {
                        return deleteCatalog;
                    }
                }
                return null;
            }
        }
    }
}
