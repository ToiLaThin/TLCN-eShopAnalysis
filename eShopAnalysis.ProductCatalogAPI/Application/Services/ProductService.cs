using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Infrastructure;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;
using System.Diagnostics.Eventing.Reader;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface IProductService
    {
        ServiceResponseDto<Product> Get(Guid productId);
        ServiceResponseDto<IEnumerable<Product>> GetAll();
        Task<ServiceResponseDto<Product>> AddProduct(Product product);
        //bool DeleteCatalog(Guid catalogId);
        //Product UpdateProduct(Product product); //for testing only
        //Product UpdateCatalogInfo(Product product);//only modify name and info

        ServiceResponseDto<Product> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName);
        ServiceResponseDto<Product> UpdateProductToOnSale(Guid productId, Guid productModelId, DiscountType discountType, double discountValue);

        //SubCatalog manipulate from product
        ServiceResponseDto<ProductModel> GetProductModel(Guid productId, Guid pModelId);
        ServiceResponseDto<IEnumerable<ProductModel>> GetAllProductModels(Guid productId);
        ServiceResponseDto<ProductModel> AddNewProductModel(Guid productId, ProductModel subCatalog);
        //SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog);
        //SubCatalog DeleteSubCatalog(Guid catalogId, Guid subCatalogId);
    }
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IBackChannelStockInventoryService _backChannelStockInventoryService;        
        public ProductService(IProductRepository productRepository, ICatalogRepository catalogRepository, IBackChannelStockInventoryService backChannelStockInventoryService)
        {
            _productRepository = productRepository;
            _catalogRepository = catalogRepository;
            _backChannelStockInventoryService = backChannelStockInventoryService;            
        }
        #region Product Services

        public ServiceResponseDto<Product> Get(Guid productId)
        {
            var result = _productRepository.Get(productId);
            if (result == null) {
              return ServiceResponseDto<Product>.Failure("productId do not match any product");
            } 
            else {
                return ServiceResponseDto<Product>.Success(result);
            }
               
        }

      public ServiceResponseDto<IEnumerable<Product>> GetAll()
        {
            var result = _productRepository.GetAll();
            return ServiceResponseDto<IEnumerable<Product>>.Success(result);
        }
        public async Task<ServiceResponseDto<Product>> AddProduct(Product product)
        {
            var result = _productRepository.Add(product);
            if (result != null)
            {
                foreach (var productModel in result.ProductModels)
                {
                    var backChannelResponse = await _backChannelStockInventoryService.AddNewStockInventory(result.ProductId.ToString(), productModel.ProductModelId.ToString(), result.BusinessKey.ToString());
                    if (backChannelResponse.IsFailed || backChannelResponse.IsException) {
                        var isDeleted = _productRepository.Delete(product); //ok
                        // retry Polly if (!isDeleted) { }
                        //if retry failed could delete all added stock of product models
                        return ServiceResponseDto<Product>.Failure(backChannelResponse.Error);
                    }
                }
                return ServiceResponseDto<Product>.Success(result);
            }
            return ServiceResponseDto<Product>.Failure("Insert product into mongo db failed");
        }

        //todo,every time update, we have to create a new row in db which is the same info but have different id
        //nen lam 1 Model ProductToUpdate vi neu update subcatalog roi update gia thi no se tao ra 2 row mới liên tục
        public ServiceResponseDto<Product> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName) {
            var productFound = _productRepository.Get(productId);
            //TODO refactor this
            var subCatalogFound = _catalogRepository.GetAllAsQueryable()
                                                    .Any(c => c.SubCatalogs.Any(sc => sc.SubCatalogId == subCatalogId && sc.SubCatalogName == subCatalogName));

            if (productFound != null && subCatalogFound is true) {
                productFound.UpdateSubCatalog(subCatalogId, subCatalogName);
                _productRepository.SaveChanges(productFound);
                return ServiceResponseDto<Product>.Success(productFound);
            }
            return ServiceResponseDto<Product>.Failure("subcatalog not found or product not found");
        }

        #endregion

        #region Product Model Services
        public ServiceResponseDto<IEnumerable<ProductModel>> GetAllProductModels(Guid productId)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return ServiceResponseDto<IEnumerable<ProductModel>>.Failure("product do not exist or not found");
            }
            var models = product.ProductModels;
            return ServiceResponseDto<IEnumerable<ProductModel>>.Success(models);
        }

        public ServiceResponseDto<ProductModel> GetProductModel(Guid productId, Guid pModelId)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return ServiceResponseDto<ProductModel>.Failure("product do not exist or not found");
            }
            var model = product.ProductModels.Where(m => m.ProductModelId == pModelId).FirstOrDefault();
            return (model == null) ? ServiceResponseDto<ProductModel>.Failure("model not found in the coresponding product") 
                : ServiceResponseDto<ProductModel>.Success(model);
        }

        public ServiceResponseDto<ProductModel> AddNewProductModel(Guid productId, ProductModel newModel)
        {
            var product = _productRepository.Get(productId);
            if (product == null)
            {
                return ServiceResponseDto<ProductModel>.Failure("product do not exist or not found");
            }
            product.AddNewProductModel(newModel);
            bool isSuccess = _productRepository.SaveChanges(product);//since we need to replace the old STATE of product with new one having new model
            if (isSuccess)
            {
                return ServiceResponseDto<ProductModel>.Success(product.ProductModels.Last());
            }
            return ServiceResponseDto<ProductModel>.Failure("cannot save changes adding product model to mongo db");
        }

        public ServiceResponseDto<Product> UpdateProductToOnSale(Guid productId, Guid productModelId, DiscountType discountType, double discountValue)
        {
            var product = _productRepository.Get(productId);
            if (product == null) { 
                return ServiceResponseDto<Product>.Failure("product cannot be found"); 
            }

            var updatedProduct = product.UpdateProductToOnSale(productModelId, discountType, discountValue);
            if (updatedProduct != null)
            {
                var result = ServiceResponseDto<Product>.Success(updatedProduct);
                _productRepository.SaveChanges(updatedProduct);
                return result;
            }
            return ServiceResponseDto<Product>.Failure("product model cannot be found or updated");
        }
        #endregion

    }
}
