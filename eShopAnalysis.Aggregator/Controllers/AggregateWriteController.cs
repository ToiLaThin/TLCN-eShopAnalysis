
using eShopAnalysis.Aggregator.Dto;
using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackChannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

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


        public AggregateWriteController(
            IBackChannelStockInventoryService backChannelStockInventoryService,
            IBackChannelCartOrderService backChannelCartOrderService,
            IBackChannelCouponSaleItemService backChannelCouponSaleItemService,
            IBackChannelProductCatalogService backChannelProductCatalogService
            )
        {
            _backChannelStockInventoryService = backChannelStockInventoryService;
            _backChannelCartOrderService = backChannelCartOrderService;
            _backChannelCouponSaleItemService = backChannelCouponSaleItemService;
            _backChannelProductCatalogService = backChannelProductCatalogService;
        }        

        [HttpPost("ApproveOrdersAndModifyStocks")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ItemStockResponseDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<ItemStockResponseDto>>> ApproveOrdersAndModifyStocks([FromBody] IEnumerable<OrderApprovedAggregate> orderApprovedAggregates)
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
                                                                     return new StockDecreaseRequestDto
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
                    return Ok(resultStockUpdate.Data);
                }
                //else use eventual consistency, rsult.isFailed mean orderstatus not set
            }
            return NotFound("resultBulkApprove or resultStockUpdate have error");

        }

        [HttpPost("CheckCouponAndAddCart")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult> CheckCouponAndAddCart([FromBody] CartConfirmRequestDto cartConfirmRequestDto)
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
        public async Task<ActionResult> AddSaleItemAndUpdateProductToOnSale([FromBody] SaleItem saleItem)
        {
            var addSaleItemBackChannelResult = await _backChannelCouponSaleItemService.AddSaleItem(saleItem);
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
    }
}
