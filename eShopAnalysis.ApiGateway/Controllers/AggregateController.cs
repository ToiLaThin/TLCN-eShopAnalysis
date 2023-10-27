using eShopAnalysis.ApiGateway.Models.Dto;
using eShopAnalysis.ApiGateway.Services.BackchannelServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ApiGateway.Controllers
{
    [Route("api/Aggregate")]
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
        [HttpGet("GetOrderToApprovedWithStock")]
        public async Task<OrderItemAndStockAggregateDto> GetOrderToApprovedWithStock()
        {
            var approvedOrdersResult = await _backChannelCartOrderService.GetToApprovedOrders();
            if (approvedOrdersResult.IsSuccess)
            {
                var allItemsInOrdersIds = approvedOrdersResult.Data.OrderItemsQty.Select(oIQ => oIQ.ProductModelId);
                var allItemsStockResult = await _backChannelStockInventoryService.GetOrderItemsStock(allItemsInOrdersIds);
                if (allItemsStockResult.IsSuccess) {
                    var orderItems = approvedOrdersResult.Data;
                    Dictionary<string, int> itemsStock = new Dictionary<string, int>();
                    foreach (var item in allItemsStockResult.Data) {
                        itemsStock.Add(item.ProductModelId.ToString(), item.CurrentQuantity);
                    }
                    return new OrderItemAndStockAggregateDto() {
                        OrderItems = new OrderItemsDto()
                        {
                            OrderId = orderItems.OrderId,
                            OrderStatus = orderItems.OrderStatus,
                            PaymentMethod = orderItems.PaymentMethod,
                            TotalPriceFinal = orderItems.TotalPriceFinal,
                            OrderItemsQty = orderItems.OrderItemsQty,
                        },
                        ItemsStock = itemsStock
                    };
                }
            }
            return null;
        }
    }
}
