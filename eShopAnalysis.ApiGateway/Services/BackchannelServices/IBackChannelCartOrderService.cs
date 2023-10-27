using eShopAnalysis.ApiGateway.Result;
using eShopAnalysis.ApiGateway.Services.BackchannelDto;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public interface IBackChannelCartOrderService
    {
        //will introduce paging later
        Task<BackChannelResponseDto<OrderItemsResponseDto>> GetToApprovedOrders(int limit = 15);
    }
}
