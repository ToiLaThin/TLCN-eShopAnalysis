using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface IProductService
    {
        Product Get(Guid productId);
        IEnumerable<Product> GetAll();
        Product AddProduct(Product product);
        //bool DeleteCatalog(Guid catalogId);
        //Product UpdateProduct(Product product); //for testing only
        //Product UpdateCatalogInfo(Product product);//only modify name and info

        //SubCatalog manipulate from product
        ProductModel GetProductModel(Guid productId, Guid pModelId);
        IEnumerable<ProductModel> GetAllProductModels(Guid productId);
        ProductModel AddNewProductModel(Guid productId, ProductModel subCatalog);
        //SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog);
        //SubCatalog DeleteSubCatalog(Guid catalogId, Guid subCatalogId);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }
        #region Product Services

        public Product Get(Guid productId)
        {
            var result = _repository.Get(productId);
            return result;
        }

        public IEnumerable<Product> GetAll()
        {
            var result = _repository.GetAll();
            return result;
        }
        public Product AddProduct(Product product)
        {

            product.AddNewProductModel(new ProductModel()); //default one which is the product itself
            var result = _repository.Add(product);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        #endregion

        #region Product Model Services
        public IEnumerable<ProductModel> GetAllProductModels(Guid productId)
        {
            var product = _repository.Get(productId);
            if (product == null)
            {
                return null;
            }
            var models = product.ProductModels;
            return models;
        }

        public ProductModel GetProductModel(Guid productId, Guid pModelId)
        {
            var product = _repository.Get(productId);
            if (product == null)
            {
                return null;
            }
            var model = product.ProductModels.Where(m => m.ProductModelId == pModelId).FirstOrDefault();
            return model;
        }

        public ProductModel AddNewProductModel(Guid productId, ProductModel newModel)
        {
            var product = _repository.Get(productId);
            if (product == null)
            {
                return null;
            }
            product.AddNewProductModel(newModel);
            bool isSuccess = _repository.SaveChanges(product);//since we need to replace the old STATE of product with new one having new model
            if (isSuccess)
            {
                return product.ProductModels.Last();
            }
            return null;
        }
        #endregion

    }
}
