using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;
using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Application.IntegrationEvents.Event;
using eShopAnalysis.ProductCatalogAPI.Application.Result;
using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Domain.Specification;
using eShopAnalysis.ProductCatalogAPI.Domain.Specification.FilterSpecification;
using eShopAnalysis.ProductCatalogAPI.Infrastructure.Contract;
using Newtonsoft.Json;
using static eShopAnalysis.ProductCatalogAPI.Domain.Models.ProductModel;

namespace eShopAnalysis.ProductCatalogAPI.Application.Services
{

    public interface IProductService
    {
        Task<ServiceResponseDto<Product>> Get(Guid productId);

        Task<ServiceResponseDto<IEnumerable<Product>>> GetAllMatching(ProductLazyLoadRequestDto lazyLoadRequestDto);

        Task<ServiceResponseDto<IEnumerable<Product>>> GetProductsWithBusinessKeys(IEnumerable<Guid> productBusinessKeys);

        Task<ServiceResponseDto<IEnumerable<Product>>> SearchProductByName(string productNameSearch);

        ServiceResponseDto<IEnumerable<Product>> GetAll();
        Task<ServiceResponseDto<Product>> AddProduct(Product product);
        //bool DeleteCatalog(Guid catalogId);
        //Product UpdateProduct(Product product); //for testing only
        //Product UpdateCatalogInfo(Product product);//only modify name and info

        Task<ServiceResponseDto<ProductModel>> UpdateProductModelPrice(Guid productId, Guid productModelId, double newPrice);

        Task<ServiceResponseDto<Product>> UpdateSubCatalog(Guid productId, Guid subCatalogId, string subCatalogName);
        Task<ServiceResponseDto<Product>> UpdateProductToOnSale(Guid productId, Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue);

        //SubCatalog manipulate from product
        Task<ServiceResponseDto<ProductModel>> GetProductModel(Guid productId, Guid pModelId);
        Task<ServiceResponseDto<IEnumerable<ProductModel>>> GetAllProductModels(Guid productId);
        Task<ServiceResponseDto<ProductModel>> AddNewProductModel(Guid productId, ProductModel subCatalog);
        //SubCatalog UpdateSubCatalog(Guid catalogId, SubCatalog subCatalog);
        //SubCatalog DeleteSubCatalog(Guid catalogId, Guid subCatalogId);

        Task<ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProvider(IEnumerable<ProductModelInfoRequestMetaDto> productModelInfoReqMetas);
        Task<ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProductModelIds(IEnumerable<Guid> productModelIds);
    }
    public class ProductService : IProductService
    {
        //unit of work can commit transaction, rollback(in case back channel service failed,
        //and mediator so we can communicate with CatalogRepo)
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;
        public ProductService(IUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
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

        public async Task<ServiceResponseDto<IEnumerable<Product>>> GetAllMatching(ProductLazyLoadRequestDto lazyLoadRequestDto)
        {            
            IQueryable<Product> originalQuery = _unitOfWork.ProductRepository.GetAllAsQueryable();
            ProductSpecificationFactory productSpecFactory = new ProductSpecificationFactory(lazyLoadRequestDto);
            var result = SpecificationEvaluator<Product>.GetQuery(query: originalQuery,
                                                                  filterSpec: productSpecFactory.FilterSpecification,
                                                                  orderSpec: productSpecFactory.OrderSpecification,
                                                                  paginateSpec: productSpecFactory.PaginateSpecification
                                                                  )
                                                        .AsEnumerable();
            return ServiceResponseDto<IEnumerable<Product>>.Success(result);
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
            _unitOfWork.CommitTransaction();
            var productAdded = await _unitOfWork.ProductRepository.GetAsync(product.ProductId);
            if (productAdded == null) {
                return ServiceResponseDto<Product>.Failure("cannot find product added, added failed or error");
            }
            return ServiceResponseDto<Product>.Success(productAdded);
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

        public async Task<ServiceResponseDto<IEnumerable<Product>>> SearchProductByName(string productNameSearch)
        {
            IQueryable<Product> originalQuery = _unitOfWork.ProductRepository.GetAllAsQueryable();
            string normalizedProductNameSearch = productNameSearch.Trim().ToLower();
            var productNameEqualFilterSpec = new ProductNameEqualOrContainFilterSpecification(normalizedProductNameSearch);
            var result = SpecificationEvaluator<Product>.GetQuery(query: originalQuery,
                                                                  filterSpec: productNameEqualFilterSpec)
                                                        .AsEnumerable();
            return ServiceResponseDto<IEnumerable<Product>>.Success(result);
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

        public async Task<ServiceResponseDto<ProductModel>> UpdateProductModelPrice(Guid productId, Guid productModelId, double newPrice)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(productId);
            ProductModel productModelToFind = product.ProductModels.Where(pm => pm.ProductModelId == productModelId).Single();
            double oldPrice = productModelToFind.Price;

            //get oldSaleItemId
            bool isToFindModelOnSale = productModelToFind.IsOnSaleModel;
            Guid? oldSaleItemId = null;
            Guid? newSaleItemId = null;
            if (isToFindModelOnSale) {
                oldSaleItemId = productModelToFind.SaleItemId;
            }
            if (product == null ) {
                return ServiceResponseDto<ProductModel>.Failure("product cannot be found");
            }

            //clone the product, then in that new product, update the product model
            var productCloned = product.Clone();
            Product productConverted = ((Product)productCloned);
            var updatedProductModel = productConverted.UpdateProductModelPrice(productModelId, newPrice);
            if (updatedProductModel == null) {
                return ServiceResponseDto<ProductModel>.Failure("product model cannot be found or updated");
            }

            //if model is on sale, we should store newSaleItemId
            if (isToFindModelOnSale) {
                newSaleItemId = updatedProductModel.SaleItemId;
            }

            //add the product to db , publish integration event, set old product to be discontinue(later)
            _unitOfWork.ProductRepository.Add(productConverted);            
            _eventBus.Publish(new ProductModelPriceUpdatedIntegrationEvent(
                oldProductId: productId,
                newProductId: productConverted.ProductId,
                oldProductModelId: productModelId,
                newProductModelId: updatedProductModel.ProductModelId,
                oldPrice: oldPrice,
                newPrice: newPrice,
                newPriceOnSaleModel: updatedProductModel.PriceOnSaleModel,
                oldSaleItemId: oldSaleItemId,
                newSaleItemId: newSaleItemId,
                productName: productConverted.ProductName
                )
            );

            var result = ServiceResponseDto<ProductModel>.Success(updatedProductModel);
            return result;

        }


        //TODO: please review & refactor this, there must be better way to do this
        public async Task<ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProvider(IEnumerable<ProductModelInfoRequestMetaDto> productModelInfoReqMetas)
        {
            if (productModelInfoReqMetas == null || productModelInfoReqMetas.Count() == 0) {
                return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Failure("invalid input params");
            }
            IEnumerable<Guid> distinctProductIds = productModelInfoReqMetas.Select(x => x.ProductId).Distinct();
            int numberOfProductModelIdBeforeDistinct = productModelInfoReqMetas.Select(x => x.ProductModelId).Count();
            //according to https://stackoverflow.com/a/5080563/23165722, this operation is O(n)
            IEnumerable<Guid> distinctProductModelIds = productModelInfoReqMetas.Select(x => x.ProductModelId).Distinct();
            if (numberOfProductModelIdBeforeDistinct != distinctProductModelIds.Count()) {
                return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Failure("product model id cannot be duplicated or reduce, it will even cause error when aggregate in read aggregate controller");
            }

            //called to db to get document of Product
            //TODO: filter to be product isUsed = true only
            var allRelatedProducts = _unitOfWork.ProductRepository.GetAllAsQueryable()
                                                                  .Where(p => distinctProductIds.Contains(p.ProductId))
                                                                  .Select(p => p)
                                                                  .ToList();

            IEnumerable<ProductModelInfoResponseDto> result = new List<ProductModelInfoResponseDto>();
            //async method in foreach: https://www.codeproject.com/Articles/5316619/Using-Asynchrony-Methods-in-Foreach-Sentences

            foreach ( var product in allRelatedProducts)
            {
                IEnumerable<ProductModel> validProductModels = product.ProductModels.Where(pm => distinctProductModelIds.Contains(pm.ProductModelId))
                                                              .Select(pm => pm);

                //this can be list since multiple model of a product is requested
                if (validProductModels.Any() == false) {
                    return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Failure("product model cannot be found even product id appear?");
                }

                IEnumerable<ProductModelInfoResponseDto> productModelInfosToAddToResult = validProductModels.Select(pm => new ProductModelInfoResponseDto
                {
                    ProductId = product.ProductId,
                    ProductModelName = product.ProductName,
                    BusinessKey = product.BusinessKey,
                    ProductModelId = pm.ProductModelId,
                    Price = pm.Price,
                    ProductCoverImage = product.ProductCoverImage
                });

                result = result.Concat(productModelInfosToAddToResult);
            }

            if (result == null || result.Count() == 0) {
                return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Failure("no valid ProductModelInfoResponseDto to return");
            }
            return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>> GetProductModelInfosOfProductModelIds(IEnumerable<Guid> productModelIds)
        {
            var productModelIdsDistinct = productModelIds.Distinct();
            var result = _unitOfWork.ProductRepository.GetAllAsQueryable()
                                                            .Where(p => productModelIdsDistinct.Contains(p.ProductModels[0].ProductModelId))
                                                            .Select(p => new ProductModelInfoResponseDto
                                                            {
                                                                ProductId = p.ProductId,
                                                                ProductModelName = p.ProductName,
                                                                BusinessKey = p.BusinessKey,
                                                                ProductModelId = p.ProductModels[0].ProductModelId,
                                                                Price = p.ProductModels[0].Price,
                                                                ProductCoverImage = p.ProductCoverImage
                                                            }).ToList();
            if (result == null || result.Count() == 0) {
                return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Failure("no valid ProductModelInfoResponseDto to return");
            }
            return ServiceResponseDto<IEnumerable<ProductModelInfoResponseDto>>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<Product>>> GetProductsWithBusinessKeys(IEnumerable<Guid> productBusinessKeys)
        {
            var result = _unitOfWork.ProductRepository.GetAllAsQueryable()
                                                      .Where(p => productBusinessKeys.Contains(p.BusinessKey))
                                                      .ToList();
            if (result == null || result.Count() == 0)
            {
                return ServiceResponseDto<IEnumerable<Product>>.Failure("no valid products to return");
            }
            return ServiceResponseDto<IEnumerable<Product>>.Success(result);
        }
        #endregion

    }
}
