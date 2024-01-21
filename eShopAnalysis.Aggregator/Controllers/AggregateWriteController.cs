
using eShopAnalysis.Aggregator.ClientDto;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ApiGateway.Controllers
{
    [Route("api/AggregateAPI/WriteAggregator")]
    [ApiController]
    public class AggregateWriteController : ControllerBase
    {
        private readonly IBackChannelCartOrderService _backChannelCartOrderService;
        private readonly IBackChannelStockInventoryService _backChannelStockInventoryService;
        private readonly IBackChannelCouponSaleItemService _backChannelCouponSaleItemService;
        private readonly IBackChannelProductCatalogService _backChannelProductCatalogService;
        private readonly IBackChannelCustomerLoyaltyProgramService _backChannelCustomerLoyaltyProgramService;
        private readonly IBackChannelStockProviderRequestService _backChannelStockProviderRequestService;


        public AggregateWriteController(
            IBackChannelStockInventoryService backChannelStockInventoryService,
            IBackChannelCartOrderService backChannelCartOrderService,
            IBackChannelCouponSaleItemService backChannelCouponSaleItemService,
            IBackChannelProductCatalogService backChannelProductCatalogService,
            IBackChannelCustomerLoyaltyProgramService backChannelCustomerLoyaltyProgramService,
            IBackChannelStockProviderRequestService backChannelStockProviderRequestService
            )
        {
            _backChannelStockInventoryService = backChannelStockInventoryService;
            _backChannelCartOrderService = backChannelCartOrderService;
            _backChannelCouponSaleItemService = backChannelCouponSaleItemService;
            _backChannelProductCatalogService = backChannelProductCatalogService;
            _backChannelCustomerLoyaltyProgramService = backChannelCustomerLoyaltyProgramService;
            _backChannelStockProviderRequestService = backChannelStockProviderRequestService;
        }        

        [HttpPost("ApproveOrdersAndModifyStocks")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<string>> ApproveOrdersAndModifyStocks([FromBody] IEnumerable<Aggregator.ClientDto.OrderApprovedAggregateDto> orderApprovedAggregates)
        {
            //convert this to sequential update stock first then update orders status , if failed then put the update
            //order status to event bus, this is more reliable, since put is idempotency
            //and this is immediate consistency first, if not then it's eventual, but becareful as this can caused that order to appear in the next batch

            if (orderApprovedAggregates == null)
                throw new ArgumentNullException(nameof(orderApprovedAggregates));

            var stockDecreaseReqs = orderApprovedAggregates.SelectMany(o => o.OrderItemsStockToChange)
                                                                 .GroupBy(req => req.ProductModelId)
                                                                 .Select(grp =>
                                                                 {
                                                                     return new Aggregator.Services.BackchannelDto.StockDecreaseRequestDto
                                                                     {
                                                                         ProductModelId = grp.Key,
                                                                         QuantityToDecrease = grp.Sum(grp => grp.QuantityToDecrease),
                                                                     };
                                                                 });
            //TODO add validate and return failed if update make stock goes below a threshhold
            var resultStockUpdate = await _backChannelStockInventoryService.DecreaseStockItems(stockDecreaseReqs);
            if (resultStockUpdate.IsSuccess)
            {
                IEnumerable<Guid> orderIdsToStockConfirmed = orderApprovedAggregates.Select(x => x.OrderId);
                var resultBulkApprove = await _backChannelCartOrderService.BulkApproveOrder(orderIdsToStockConfirmed);
                if (resultBulkApprove.IsSuccess)
                {
                    return Ok("ApproveOrdersAndModifyStocks succeeded");
                }
                //else use eventual consistency, rsult.isFailed mean orderstatus not set
            }
            return NotFound("resultBulkApprove or resultStockUpdate have error");

        }

        [HttpPost("CheckCouponAndAddCart")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult> CheckCouponAndAddCart([FromBody] Aggregator.ClientDto.CartConfirmRequestDto cartConfirmRequestDto)
        {
            CartConfirmRequestToCartApiDto requestToCartApiDto = new CartConfirmRequestToCartApiDto
            {
                CartItems = cartConfirmRequestDto.CartItems,
                UserId = cartConfirmRequestDto.UserId,
                Coupon = null
            };

            //if have coupon in request frontend, try get the coupon data and set in the request to cart add
            if (cartConfirmRequestDto.CouponCode != null && !cartConfirmRequestDto.CouponCode.Equals(String.Empty))
            {
                var backChannelResponse = await _backChannelCouponSaleItemService.RetrieveCouponWithCode(cartConfirmRequestDto.CouponCode);
                if (backChannelResponse.IsFailed || backChannelResponse.IsException)
                {
                    return NotFound(backChannelResponse.Error);
                }
                requestToCartApiDto.Coupon = backChannelResponse.Data;

                //and if the coupon is valid we must also change the reward point if reward point in coupon > 0
                if (backChannelResponse.Data.RewardPointRequire > 0) {
                    CouponDiscountType couponDiscountType = RewardTransactionHelperAdapter.ToCouponDiscountTypeAdapter(backChannelResponse.Data.DiscountType);
                    RewardTransactionForApplyCouponAddRequestDto requestDto = new RewardTransactionForApplyCouponAddRequestDto()
                    {
                        UserId = cartConfirmRequestDto.UserId,
                        //mismatch type between 2 enum: DiscountType & CouponDiscountType => need adapter helper method
                        DiscountType = couponDiscountType,
                        DiscountValue = backChannelResponse.Data.DiscountValue,
                        PointTransition = (-1) * backChannelResponse.Data.RewardPointRequire //because the controller require point to < 0
                    };
                    var rewardTransBackChannelResp = await _backChannelCustomerLoyaltyProgramService.AddRewardTransactionForApplyCoupon(requestDto);
                    //if NOT success eventual consistency
                }
                
            }

            //if no coupon apply, the request to cart api will have no coupon
            var cartBackChannelResp = await _backChannelCartOrderService.AddCart(requestToCartApiDto);
            if (cartBackChannelResp.IsFailed || cartBackChannelResp.IsException)
            {
                return NotFound(cartBackChannelResp.Error);
            }
            return Ok();
        }

        [HttpPost("AddSaleItemAndUpdateProductToOnSale")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult> AddSaleItemAndUpdateProductToOnSale([FromBody] Aggregator.ClientDto.SaleItemDto saleItemDto)
        {
            //could use automapper here, this part show that model with same name is different
            Aggregator.Services.BackchannelDto.SaleItemDto backChannelSaleItemDto = new()
            {
                SaleItemId = saleItemDto.SaleItemId,
                ProductId = saleItemDto.ProductId,
                ProductModelId = saleItemDto.ProductModelId,
                BusinessKey = saleItemDto.BusinessKey,
                DateAdded = saleItemDto.DateAdded,
                DateEnded = saleItemDto.DateEnded,
                SaleItemStatus = saleItemDto.SaleItemStatus,
                DiscountType = saleItemDto.DiscountType,
                DiscountValue = saleItemDto.DiscountValue,
                RewardPointRequire = saleItemDto.RewardPointRequire,
            };
            var addSaleItemBackChannelResult = await _backChannelCouponSaleItemService.AddSaleItem(backChannelSaleItemDto);
            if (addSaleItemBackChannelResult.IsFailed || addSaleItemBackChannelResult.IsException)
            {
                return NotFound(addSaleItemBackChannelResult.Error);
            }
            var saleItemAdded = addSaleItemBackChannelResult.Data;
            var updateProductToSaleBackChannelResult = await _backChannelProductCatalogService.UpdateProductToSaleAsync(productId: saleItemAdded.ProductId,
                                                                                                                        productModelId: saleItemAdded.ProductModelId,
                                                                                                                        saleItemId: saleItemAdded.SaleItemId,
                                                                                                                        discountType: saleItemAdded.DiscountType,
                                                                                                                        discountValue: saleItemAdded.DiscountValue);
            if (updateProductToSaleBackChannelResult.IsFailed || updateProductToSaleBackChannelResult.IsException)
            {
                throw new Exception("testing, but this is critial error");
                return NotFound(updateProductToSaleBackChannelResult.Error);
                //TODO can use compensation trans to delete added saleItem or use polly to retry since this is idempotency
            }
            return Ok();

        }

        [HttpPost("AddNewProductAndModelsStock")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult> AddNewProductAndModelsStock([FromBody] ProductDto newProductDto)
        {
            if (newProductDto == null)
            {
                throw new ArgumentNullException(nameof(newProductDto));
            }

            var addProductBackChannelRes = await _backChannelProductCatalogService.AddProduct(newProductDto);
            if (addProductBackChannelRes.IsFailed || addProductBackChannelRes.IsException)
            {
                return NotFound(addProductBackChannelRes.Error);
            }

            string productAddedId = addProductBackChannelRes.Data.ProductId.ToString();
            string productAddedBusinessKey = addProductBackChannelRes.Data.BusinessKey.ToString();
            IEnumerable<StockInventoryDto> stockInventoryDtos = addProductBackChannelRes.Data.ProductModels.Select(pm => new StockInventoryDto()
            {
                ProductId = productAddedId,
                ProductModelId = pm.ProductModelId.ToString(),
                ProductBusinessKey = productAddedBusinessKey
            });
            var addStocksBackChannelRes = await _backChannelStockInventoryService.AddNewStockInventories(stockInventoryDtos);
            if (addStocksBackChannelRes.IsFailed || addStocksBackChannelRes.IsException)
            {
                throw new Exception("testing, but this is critial error");
                //TODO can use compensation trans to delete added product or use polly to retry since this is idempotency(delete can be idempotency with check)
            }
            return Ok(addProductBackChannelRes.Data);
        }

        [HttpPost("AddStockReqTransAndIncreaseStockItems")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StockRequestTransactionDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult> AddStockReqTransAndIncreaseStockItems([FromBody] StockRequestTransactionDto stockReqTrans)
        {
            if (stockReqTrans == null) {
                throw new ArgumentNullException(nameof(stockReqTrans));
            }

            stockReqTrans.TotalQuantity = stockReqTrans.StockItemRequests.Aggregate(0, (sumAllStockItemsQty, stockItemReq) => sumAllStockItemsQty + stockItemReq.ItemQuantity);
            stockReqTrans.TotalTransactionPrice = stockReqTrans.StockItemRequests.Aggregate(0.0, (sumAllStockItemsPrice, stockItemReq) => sumAllStockItemsPrice + stockItemReq.UnitRequestPrice);
            var addStockReqTransBackChannelRes = await _backChannelStockProviderRequestService.AddNewStockRequestTransaction(stockReqTrans);
            if (addStockReqTransBackChannelRes.IsFailed || addStockReqTransBackChannelRes.IsException) {
                return NotFound(addStockReqTransBackChannelRes.Error);
            }

            var stockIncreaseReqs = stockReqTrans.StockItemRequests.Select(stockItemReq => new StockIncreaseRequestDto(stockItemReq.ProductModelId, stockItemReq.ItemQuantity));
            var stockIncreaseBackChannelRes = await _backChannelStockInventoryService.IncreaseStockItems(stockIncreaseReqs);
            if (stockIncreaseBackChannelRes.IsFailed || stockIncreaseBackChannelRes.IsException) {
                throw new Exception("testing, but this is critial error");
                //TODO can use compensation trans to delete added stock request trans or use polly to retry since this is idempotency(delete can be idempotency with check)
            }
            return Ok(addStockReqTransBackChannelRes.Data);
        }
    }
}
