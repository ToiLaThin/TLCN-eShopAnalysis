using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using System.Diagnostics.Eventing.Reader;

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

        Product UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName);

        //SubCatalog manipulate from product
        ProductModel GetProductModel(Guid productId, Guid pModelId);
        IEnumerable<ProductModel> GetAllProductModels(Guid productId);
        ProductModel AddNewProductModel(Guid productId, ProductModel subCatalog);
        //SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog);
        //SubCatalog DeleteSubCatalog(Guid catalogId, Guid subCatalogId);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICatalogRepository _catalogRepository;
        public ProductService(IProductRepository productRepository, ICatalogRepository catalogRepository)
        {
            _productRepository = productRepository;
            _catalogRepository = catalogRepository;
        }
        #region Product Services

        public Product Get(Guid productId)
        {
            var result = _productRepository.Get(productId);
            return result;
        }

        public IEnumerable<Product> GetAll()
        {
            var result = _productRepository.GetAll();
            return result;
        }
        public Product AddProduct(Product product)
        {
            product.AddNewProductModel(new ProductModel()); //default one which is the product itself
            var result = _productRepository.Add(product);
            if (result != null)
            {
                return result;
            }
            return null;
        }

        //todo,every time update, we have to create a new row in db which is the same info but have different id
        //nen lam 1 Model ProductToUpdate vi neu update subcatalog roi update gia thi no se tao ra 2 row mới liên tục
        public Product UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName) {
            var productFound = _productRepository.Get(productId);
            //TODO refactor this
            var subCatalogFound = _catalogRepository.GetAllAsQueryable()
                                                    .Any(c => c.SubCatalogs.Any(sc => sc.SubCatalogId == subCatalogId && sc.SubCatalogName == subCatalogName));

            if (productFound != null && subCatalogFound is true) {
                productFound.UpdateSubCatalog(subCatalogId, subCatalogName);
                _productRepository.SaveChanges(productFound);
                return productFound;
            }
            return new Product();
        }

        #endregion

        #region Product Model Services
        public IEnumerable<ProductModel> GetAllProductModels(Guid productId)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return null;
            }
            var models = product.ProductModels;
            return models;
        }

        public ProductModel GetProductModel(Guid productId, Guid pModelId)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return null;
            }
            var model = product.ProductModels.Where(m => m.ProductModelId == pModelId).FirstOrDefault();
            return model;
        }

        public ProductModel AddNewProductModel(Guid productId, ProductModel newModel)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return null;
            }
            product.AddNewProductModel(newModel);
            bool isSuccess = _productRepository.SaveChanges(product);//since we need to replace the old STATE of product with new one having new model
            if (isSuccess)
            {
                return product.ProductModels.Last();
            }
            return null;
        }
        #endregion

    }
}
