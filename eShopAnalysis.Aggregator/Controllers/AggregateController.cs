
using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Services.BackchannelServices;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ApiGateway.Controllers
{
    [Route("api/AggregateAPI/AggregateOrderItemStock")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly IBackChannelCartOrderService _backChannelCartOrderService;
        private readonly IBackChannelStockInventoryService _backChannelStockInventoryService;

        public AggregateController(
            IBackChannelStockInventoryService backChannelStockInventoryService,
            IBackChannelCartOrderService backChannelCartOrderService
            )
        {
            _backChannelStockInventoryService = backChannelStockInventoryService;
            _backChannelCartOrderService = backChannelCartOrderService;
        }
        [HttpGet("GetOrderToApproveWithStock")]
        public async Task<OrderItemAndStockAggregateDto> GetOrderToApprovedWithStock()
        {
            var approvedOrdersResult = await _backChannelCartOrderService.GetToApprovedOrders();
            if (approvedOrdersResult.IsSuccess)
            {
                //https://stackoverflow.com/a/34883995
                var allItemsInOrdersIds = approvedOrdersResult.Data.SelectMany(o => o.OrderItemsQty)
                                                                   .DistinctBy(oIQ => oIQ.ProductModelId)
                                                                   .Select(oIQ => oIQ.ProductModelId)
                                                                   .ToList();
                var allItemsStockResult = await _backChannelStockInventoryService.GetOrderItemsStock(allItemsInOrdersIds);
                if (allItemsStockResult.IsSuccess) {
                    var ordersToApprovedResp = approvedOrdersResult.Data;
                    Dictionary<string, int> itemsStock = new Dictionary<string, int>();
                    foreach (var item in allItemsStockResult.Data) {
                        itemsStock.Add(item.ProductModelId.ToString(), item.CurrentQuantity);
                    }

                    var ordersToApproved = new List<OrderItemsDto>();
                    foreach (var orderResp in  ordersToApprovedResp)
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
                    return new OrderItemAndStockAggregateDto() {
                        OrderItems = ordersToApproved,
                        ItemsStock = itemsStock
                    };
                }
            }
            return null;
        }
    }
}
