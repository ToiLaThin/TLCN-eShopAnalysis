
using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelCartOrderService
    {
        //will introduce paging later
        Task<BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit = 15);
    }
}
