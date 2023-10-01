using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using System.Diagnostics.Eventing.Reader;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface IProductService
    {
        ResponseDto<Product> Get(Guid productId);
        ResponseDto<IEnumerable<Product>> GetAll();
        ResponseDto<Product> AddProduct(Product product);
        //bool DeleteCatalog(Guid catalogId);
        //Product UpdateProduct(Product product); //for testing only
        //Product UpdateCatalogInfo(Product product);//only modify name and info

        ResponseDto<Product> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName);

        //SubCatalog manipulate from product
        ResponseDto<ProductModel> GetProductModel(Guid productId, Guid pModelId);
        ResponseDto<IEnumerable<ProductModel>> GetAllProductModels(Guid productId);
        ResponseDto<ProductModel> AddNewProductModel(Guid productId, ProductModel subCatalog);
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

        public ResponseDto<Product> Get(Guid productId)
        {
            var result = _productRepository.Get(productId);
            if (result == null) {
              return ResponseDto<Product>.Failure("productId do not match any product");
            } 
            else {
                return ResponseDto<Product>.Success(result);
            }
               
        }

      public ResponseDto<IEnumerable<Product>> GetAll()
        {
            var result = _productRepository.GetAll();
            return ResponseDto<IEnumerable<Product>>.Success(result);
        }
        public ResponseDto<Product> AddProduct(Product product)
        {
            var result = _productRepository.Add(product);
            if (result != null)
            {
                return ResponseDto<Product>.Success(result);
            }
            return ResponseDto<Product>.Failure("Insert product into mongo db failed");
        }

        //todo,every time update, we have to create a new row in db which is the same info but have different id
        //nen lam 1 Model ProductToUpdate vi neu update subcatalog roi update gia thi no se tao ra 2 row mới liên tục
        public ResponseDto<Product> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName) {
            var productFound = _productRepository.Get(productId);
            //TODO refactor this
            var subCatalogFound = _catalogRepository.GetAllAsQueryable()
                                                    .Any(c => c.SubCatalogs.Any(sc => sc.SubCatalogId == subCatalogId && sc.SubCatalogName == subCatalogName));

            if (productFound != null && subCatalogFound is true) {
                productFound.UpdateSubCatalog(subCatalogId, subCatalogName);
                _productRepository.SaveChanges(productFound);
                return ResponseDto<Product>.Success(productFound);
            }
            return ResponseDto<Product>.Failure("subcatalog not found or product not found");
        }

        #endregion

        #region Product Model Services
        public ResponseDto<IEnumerable<ProductModel>> GetAllProductModels(Guid productId)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return ResponseDto<IEnumerable<ProductModel>>.Failure("product do not exist or not found");
            }
            var models = product.ProductModels;
            return ResponseDto<IEnumerable<ProductModel>>.Success(models);
        }

        public ResponseDto<ProductModel> GetProductModel(Guid productId, Guid pModelId)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return ResponseDto<ProductModel>.Failure("product do not exist or not found");
            }
            var model = product.ProductModels.Where(m => m.ProductModelId == pModelId).FirstOrDefault();
            return (model == null) ? ResponseDto<ProductModel>.Failure("model not found in the coresponding product") 
                : ResponseDto<ProductModel>.Success(model);
        }

        public ResponseDto<ProductModel> AddNewProductModel(Guid productId, ProductModel newModel)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return ResponseDto<ProductModel>.Failure("product do not exist or not found");
            }
            product.AddNewProductModel(newModel);
            bool isSuccess = _productRepository.SaveChanges(product);//since we need to replace the old STATE of product with new one having new model
            if (isSuccess)
            {
                return ResponseDto<ProductModel>.Success(product.ProductModels.Last());
            }
            return ResponseDto<ProductModel>.Failure("cannot save changes adding product model to mongo db");
        }
        #endregion

    }
}
