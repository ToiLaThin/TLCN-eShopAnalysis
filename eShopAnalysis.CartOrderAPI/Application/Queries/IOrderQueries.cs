using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public interface IOrderQueries
    {
        //dapper ko co change tracker, neu implement specification pattern cung kho hon
        Task<QueryResponseDto<IEnumerable<OrderDraftViewModel>>> GetUserDraftOrders(Guid userId);
        Task<QueryResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit);
    }
}
