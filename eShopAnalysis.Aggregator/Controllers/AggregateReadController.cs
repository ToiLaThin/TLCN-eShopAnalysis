using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using eShopAnalysis.Aggregator.Utilities.Behaviors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        public AggregateReadController(
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

        [HttpGet("GetOrderToApproveWithStock")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OrderItemAndStockAggregateDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderItemAndStockAggregateDto>> GetOrderToApprovedWithStock()
        {
            var approvedOrdersResult = await _backChannelCartOrderService.GetToApprovedOrders();
            if (approvedOrdersResult.IsSuccess && approvedOrdersResult.Data.Count() > 0)
            {
                //https://stackoverflow.com/a/34883995
                var allItemsInOrdersIds = approvedOrdersResult.Data.SelectMany(o => o.OrderItemsQty)
                                                                   .DistinctBy(oIQ => oIQ.ProductModelId)
                                                                   .Select(oIQ => oIQ.ProductModelId)
                                                                   .ToList();
                var allItemsStockResult = await _backChannelStockInventoryService.GetOrderItemsStock(allItemsInOrdersIds);
                if (allItemsStockResult.IsSuccess)
                {
                    var ordersToApprovedResp = approvedOrdersResult.Data;
                    Dictionary<string, int> itemsStock = new Dictionary<string, int>();
                    foreach (var item in allItemsStockResult.Data)
                    {
                        itemsStock.Add(item.ProductModelId.ToString(), item.CurrentQuantity);
                    }

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
                    var result = new OrderItemAndStockAggregateDto()
                    {
                        OrderItems = ordersToApproved,
                        ItemsStock = itemsStock
                    };
                    return Ok(result);
                }
            }
            return NotFound("GetToApprovedOrders failed or GetOrderItemsStock failed");
        }
    }
}
