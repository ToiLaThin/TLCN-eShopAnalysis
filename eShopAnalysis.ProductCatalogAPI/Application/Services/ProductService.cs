using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Domain.Specification;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract;
using static eShopAnalysis.ProductCatalogAPI.Domain.Models.ProductModel;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface IProductService
    {
        Task<ServiceResponseDto<Product>> Get(Guid productId);
        ServiceResponseDto<IEnumerable<Product>> GetAll();
        Task<ServiceResponseDto<Product>> AddProduct(Product product);
        //bool DeleteCatalog(Guid catalogId);
        //Product UpdateProduct(Product product); //for testing only
        //Product UpdateCatalogInfo(Product product);//only modify name and info

        Task<ServiceResponseDto<Product>> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName);
        Task<ServiceResponseDto<Product>> UpdateProductToOnSale(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue);

        //SubCatalog manipulate from product
        Task<ServiceResponseDto<ProductModel>> GetProductModel(Guid productId, Guid pModelId);
        Task<ServiceResponseDto<IEnumerable<ProductModel>>> GetAllProductModels(Guid productId);
        Task<ServiceResponseDto<ProductModel>> AddNewProductModel(Guid productId, ProductModel subCatalog);
        //SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog);
        //SubCatalog DeleteSubCatalog(Guid catalogId, Guid subCatalogId);
    }
    public class ProductService : IProductService
    {
        //unit of work can commit transaction, rollback(in case back channel service failed,
        //and mediator so we can communicate with CatalogRepo)
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackChannelStockInventoryService _backChannelStockInventoryService;        
        public ProductService(IUnitOfWork unitOfWork, IBackChannelStockInventoryService backChannelStockInventoryService)
        {
            _unitOfWork = unitOfWork;
            _backChannelStockInventoryService = backChannelStockInventoryService;            
        }
        #region Product Services

        public async Task<ServiceResponseDto<Product>> Get(Guid productId)
        {
            var result = await _unitOfWork.ProductRepository.GetAsync(productId);
            if (result == null) {
              return ServiceResponseDto<Product>.Failure("productId do not match any product");
            } 
            else {
                return ServiceResponseDto<Product>.Success(result);
            }
               
        }

      public ServiceResponseDto<IEnumerable<Product>> GetAll()
        {
            var result = _unitOfWork.ProductRepository.GetAllAsQueryable().ToList();
            return ServiceResponseDto<IEnumerable<Product>>.Success(result);

            //https://www.mongodb.com/docs/realm/sdk/dotnet/crud/filter/ , i used multiple where clause 
            //if we use expression tree, have error in SpecificationEvaluator, cannot parse the expression, only on in another provider
            //IQueryable<Product> originalQuery = _unitOfWork.ProductRepository.GetAllAsQueryable();
            //var cublicFilterSpecByM = new CublicTypeFilterSpecification(CublicType.M);
            //var cublicFilterSpecByV = new CublicTypeFilterSpecification(CublicType.V);
            //var onSaleFilterSpec = new OnSaleFilterSpecification();
            //var salePriceInRangFilterSpec = new InRangeSalePriceFilterSpecification(2, 13);
            //var finalCublicFilter = salePriceInRangFilterSpec.And(onSaleFilterSpec).And(salePriceInRangFilterSpec);

            //var orderSpec = new SaleDisplayPriceOrderSpecification(OrderType.Ascending);
            //var paginateSpec = new PaginateSpecification(1, 1);
            //var result = SpecificationEvaluator<Product>.GetQuery(originalQuery, finalCublicFilter, orderSpec)
            //                                            .AsEnumerable();
            //return ServiceResponseDto<IEnumerable<Product>>.Success(result);
        }
        public async Task<ServiceResponseDto<Product>> AddProduct(Product product)
        {
            _unitOfWork.BeginTransactionAsync();
            //pass in the sessionHandle to add product in the current transaction 
            await _unitOfWork.ProductRepository.AddAsync(product, _unitOfWork.GetClientSessionHandle());

            foreach (var productModel in product.ProductModels) {
                var backChannelResponse = await _backChannelStockInventoryService.AddNewStockInventory(product.ProductId.ToString(), 
                                                                                                       productModel.ProductModelId.ToString(),
                                                                                                       product.BusinessKey.ToString());
                if (backChannelResponse.IsFailed || backChannelResponse.IsException) {
                    _unitOfWork.RollbackTransaction();//do not need to delete the add product
                    return ServiceResponseDto<Product>.Failure(backChannelResponse.Error);
                }
            }
            _unitOfWork.CommitTransaction();
            return ServiceResponseDto<Product>.Success(product);
        }

        //todo,every time update, we have to create a new row in db which is the same info but have different id
        //nen lam 1 Model ProductToUpdate vi neu update subcatalog roi update gia thi no se tao ra 2 row mới liên tục
        public async Task<ServiceResponseDto<Product>> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName) {
            var productFound = await _unitOfWork.ProductRepository.GetAsync(productId);
            var subCatalogFound = _unitOfWork.CatalogRepository.GetAllAsQueryable()
                                                    .Any(c => c.SubCatalogs.Any(sc => sc.SubCatalogId == subCatalogId && sc.SubCatalogName == subCatalogName));

            if (productFound != null && subCatalogFound is true) {
                productFound.UpdateSubCatalog(subCatalogId, subCatalogName);
                await _unitOfWork.ProductRepository.SaveChangesAsync(productFound);
                return ServiceResponseDto<Product>.Success(productFound);
            }
            return ServiceResponseDto<Product>.Failure("subcatalog not found or product not found");
        }

        #endregion

        #region Product Model Services
        public async Task<ServiceResponseDto<IEnumerable<ProductModel>>> GetAllProductModels(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(productId);
            if (product == null) {
                return ServiceResponseDto<IEnumerable<ProductModel>>.Failure("product do not exist or not found");
            }
            var models = product.ProductModels;
            return ServiceResponseDto<IEnumerable<ProductModel>>.Success(models);
        }

        public async Task<ServiceResponseDto<ProductModel>> GetProductModel(Guid productId, Guid pModelId)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(productId);
            if (product == null) {
                return ServiceResponseDto<ProductModel>.Failure("product do not exist or not found");
            }
            var model = product.ProductModels.Where(m => m.ProductModelId == pModelId).FirstOrDefault();
            return (model == null) ? ServiceResponseDto<ProductModel>.Failure("model not found in the coresponding product") 
                : ServiceResponseDto<ProductModel>.Success(model);
        }

        public async Task<ServiceResponseDto<ProductModel>> AddNewProductModel(Guid productId, ProductModel newModel)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(productId);
            if (product == null) {
                return ServiceResponseDto<ProductModel>.Failure("product do not exist or not found");
            }
            product.AddNewProductModel(newModel);
            bool isSuccess = await _unitOfWork.ProductRepository.SaveChangesAsync(product);//since we need to replace the old STATE of product with new one having new model
            if (isSuccess) {
                return ServiceResponseDto<ProductModel>.Success(product.ProductModels.Last());
            }
            return ServiceResponseDto<ProductModel>.Failure("cannot save changes adding product model to mongo db");
        }

        public async Task<ServiceResponseDto<Product>> UpdateProductToOnSale(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(productId);
            if (product == null) { 
                return ServiceResponseDto<Product>.Failure("product cannot be found"); 
            }

            var updatedProduct = product.UpdateProductToOnSale(productModelId, saleItemId, discountType, discountValue);
            if (updatedProduct == null) {
                return ServiceResponseDto<Product>.Failure("product model cannot be found or updated");
            }
            var result = ServiceResponseDto<Product>.Success(updatedProduct);
            await _unitOfWork.ProductRepository.SaveChangesAsync(updatedProduct);
            return result;
        }
        #endregion

    }
}
