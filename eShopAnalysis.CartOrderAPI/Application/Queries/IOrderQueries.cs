using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public interface IOrderQueries
    {
        Task<IEnumerable<OrderDraftViewModel>> GetUserDraftOrders(Guid userId);
        Task<IEnumerable<OrderItemsResponseDto>> GetToApprovedOrders(int limit);
    }
}
