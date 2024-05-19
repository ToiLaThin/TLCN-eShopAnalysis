using eShopAnalysis.Aggregator.ClientDto;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities.Behaviors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace eShopAnalysis.Aggregator.Controllers
{
    [Route("api/AggregateAPI/ReadAggregator")]
    [ApiController]
    public class AggregateReadController : ControllerBase
    {
        private readonly IBackChannelCartOrderService _backChannelCartOrderService;
        private readonly IBackChannelStockInventoryService _backChannelStockInventoryService;
        private readonly IBackChannelCouponSaleItemService _backChannelCouponSaleItemService;
        private readonly IBackChannelProductCatalogService _backChannelProductCatalogService;
        private readonly IBackChannelStockProviderRequestService _backChannelStockProviderRequestService;

        public AggregateReadController(
            IBackChannelStockInventoryService backChannelStockInventoryService,
            IBackChannelCartOrderService backChannelCartOrderService,
            IBackChannelCouponSaleItemService backChannelCouponSaleItemService,
            IBackChannelProductCatalogService backChannelProductCatalogService,
            IBackChannelStockProviderRequestService backChannelStockProviderRequestService
            )
        {
            _backChannelStockInventoryService = backChannelStockInventoryService;
            _backChannelCartOrderService = backChannelCartOrderService;
            _backChannelCouponSaleItemService = backChannelCouponSaleItemService;
            _backChannelProductCatalogService = backChannelProductCatalogService;
            _backChannelStockProviderRequestService = backChannelStockProviderRequestService;
        }

        [HttpGet("GetOrderToApproveWithStock")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OrderItemAndStockLookupAggregateDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderItemAndStockLookupAggregateDto>> GetOrderToApprovedWithStock()
        {
            var approvedOrdersResult = await _backChannelCartOrderService.GetToApprovedOrders();
            if (approvedOrdersResult.IsFailed || approvedOrdersResult.IsException || approvedOrdersResult.Data.Count() <= 0) {
                return NotFound("GetToApprovedOrders failed");
            }

            //https://stackoverflow.com/a/34883995
            var allItemsInOrdersIds = approvedOrdersResult.Data.SelectMany(o => o.OrderItemsQty)
                                                                .DistinctBy(oIQ => oIQ.ProductModelId)
                                                                .Select(oIQ => oIQ.ProductModelId)
                                                                .ToList();
            var productModelInfosResult = await _backChannelProductCatalogService.GetProductModelInfosOfProductModelIds(allItemsInOrdersIds);
            if (productModelInfosResult.IsFailed || productModelInfosResult.IsException) { 
                return NotFound("GetProductModelInfosOfProductModelIds failed");
            }

            var allItemsStockResult = await _backChannelStockInventoryService.GetOrderItemsStock(allItemsInOrdersIds);
            if (allItemsStockResult.IsFailed || allItemsStockResult.IsException) {
                return NotFound("GetOrderItemsStock failed");
            }

            var stockItemReqMetasFromProvidersResult = await _backChannelStockProviderRequestService.GetStockItemRequestMetasWithProductModelIds(allItemsInOrdersIds);
            if (stockItemReqMetasFromProvidersResult.IsFailed || stockItemReqMetasFromProvidersResult.IsException) {
                return NotFound("GetStockItemRequestMetasWithProductModelIds failed");
            }

            var productModelInfoResponses = productModelInfosResult.Data;
            var allItemsStockResponses = allItemsStockResult.Data;
            var ordersToApprovedResp = approvedOrdersResult.Data;
            var stockItemReqMetaContainingPModelIds = stockItemReqMetasFromProvidersResult.Data;
            IEnumerable<ProductModelInfoWithStockAggregateDto> productModelInfoWithStockAggregates = (
                from pMI in productModelInfoResponses
                join iS in allItemsStockResponses on pMI.ProductModelId equals iS.ProductModelId
                join sIRM in stockItemReqMetaContainingPModelIds on pMI.ProductModelId equals sIRM.ProductModelId
                select new ProductModelInfoWithStockAggregateDto()
                {
                    ProductModelId = pMI.ProductModelId,
                    ProductId = pMI.ProductId,
                    BusinessKey = pMI.BusinessKey,
                    ProductModelName = pMI.ProductModelName,
                    ProductCoverImage = pMI.ProductCoverImage,
                    Price = pMI.Price,
                    UnitRequestPrice = sIRM.UnitRequestPrice,
                    CurrentQuantity = iS.CurrentQuantity,
                    QuantityToRequestMoreFromProvider = sIRM.QuantityToRequestMoreFromProvider,
                    QuantityToNotify = sIRM.QuantityToNotify
                });

            var ordersToApproved = new List<OrderItemsDto>();
            foreach (var orderResp in ordersToApprovedResp)
            {
                ordersToApproved.Add(new OrderItemsDto()
                {
                    OrderId = orderResp.OrderId,
                    OrderStatus = orderResp.OrderStatus,
                    PaymentMethod = orderResp.PaymentMethod,
                    TotalPriceFinal = orderResp.TotalPriceFinal,
                    OrderItemsQty = orderResp.OrderItemsQty,
                });
            }
            var result = new OrderItemAndStockLookupAggregateDto()
            {
                OrderItems = ordersToApproved,
                StockLookupItems = productModelInfoWithStockAggregates
            };
            return Ok(result);
        }

        //post but to get only
        [HttpPost("GetProductModelInfosWithStockOfProvider")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProductModelInfoWithStockAggregateDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<ProductModelInfoWithStockAggregateDto>>> GetProductModelInfosWithStockOfProvider([FromBody] IEnumerable<ProductModelInfoRequestMetaDto> productModelInfoRequestMetas)
        {
            IEnumerable<Guid> allProviderProductModelIds = productModelInfoRequestMetas.Select(pMIRM => pMIRM.ProductModelId).ToList();

            var productModelInfosResult = await _backChannelProductCatalogService.GetProductModelInfosOfProvider(productModelInfoRequestMetas);
            if (productModelInfosResult.IsFailed || productModelInfosResult.IsException) {
                return NotFound(productModelInfosResult.Error);
            }
            var itemsStockResult = await _backChannelStockInventoryService.GetOrderItemsStock(allProviderProductModelIds);//ItemStockRequestDto will be create inside here
            if (itemsStockResult.IsFailed || itemsStockResult.IsException) {
                return NotFound(itemsStockResult.Error);
            }

            IEnumerable<ItemStockResponseDto> itemStockResponses = itemsStockResult.Data;
            IEnumerable<ProductModelInfoResponseDto> productModelInfoResponses = productModelInfosResult.Data;

            //must use query syntax to join three IEnumerable
            IEnumerable<ProductModelInfoWithStockAggregateDto> productModelInfoWithStockAggregates = (
                from pMI in productModelInfoResponses
                join iS in itemStockResponses on pMI.ProductModelId equals iS.ProductModelId
                join pMIReqMeta in productModelInfoRequestMetas on pMI.ProductModelId equals pMIReqMeta.ProductModelId
                select new ProductModelInfoWithStockAggregateDto()
                {
                    ProductModelId = pMI.ProductModelId,
                    ProductId = pMI.ProductId,
                    BusinessKey = pMI.BusinessKey,
                    ProductModelName = pMI.ProductModelName,
                    ProductCoverImage = pMI.ProductCoverImage,
                    Price = pMI.Price,
                    UnitRequestPrice = pMIReqMeta.UnitRequestPrice,
                    CurrentQuantity = iS.CurrentQuantity,
                    QuantityToRequestMoreFromProvider = pMIReqMeta.QuantityToRequestMoreFromProvider,
                    QuantityToNotify = pMIReqMeta.QuantityToNotify
                });
            if (productModelInfoWithStockAggregates == null || productModelInfoWithStockAggregates.Count() <=0 ) {
                return NotFound("join result is null");
            }
            return Ok(productModelInfoWithStockAggregates);
        }
    }


}
