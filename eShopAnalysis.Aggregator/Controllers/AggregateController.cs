
using eShopAnalysis.Aggregator.Models.Dto;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
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


        [HttpPost("ApproveOrdersAndModifyStocks")]
        public async Task<IEnumerable<ItemStockResponseDto>> ApproveOrdersAndModifyStocks([FromBody] IEnumerable<OrderApprovedAggregate> orderApprovedAggregates)
        {
            //convert this to sequential update stock first then update orders status , if failed then put the update
            //order status to event bus, this is more reliable, since put is idempotency
            //and this is immediate consistency first, if not then it's eventual, but becareful as this can caused that order to appear in the next batch

            if (orderApprovedAggregates == null)
                throw new ArgumentNullException(nameof(orderApprovedAggregates));

            var stockDecreaseReqs = orderApprovedAggregates.SelectMany(o => o.OrderItemsStockToChange)
                                                                 .GroupBy(req => req.ProductModelId)
                                                                 .Select(grp => {
                                                                     return new StockDecreaseRequestDto
                                                                     {
                                                                         ProductModelId = grp.Key,
                                                                         QuantityToDecrease = grp.Sum(grp => grp.QuantityToDecrease),
                                                                     };
                                                                 });
            //TODO add validate and return failed if update make stock goes below a threshhold
            var resultStockUpdate = await _backChannelStockInventoryService.DecreaseStockItems(stockDecreaseReqs);
            if (resultStockUpdate.IsSuccess) {
                IEnumerable<Guid> orderIdsToStockConfirmed = orderApprovedAggregates.Select(x => x.OrderId);
                var resultBulkApprove = await _backChannelCartOrderService.BulkApproveOrder(orderIdsToStockConfirmed);
                if (resultBulkApprove.IsSuccess) {
                    return resultStockUpdate.Data;
                }
                //else use eventual consistency, rsult.isFailed mean orderstatus not set
            }
            return null;

        }
    }
}
