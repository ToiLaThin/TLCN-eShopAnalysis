using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;

namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public interface IOrderQueries
    {
        //dapper ko co change tracker, neu implement specification pattern cung kho hon
        Task<QueryResponseDto<IEnumerable<OrderDraftViewModel>>> GetUserDraftOrders(Guid userId);
        Task<QueryResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit);

        Task<QueryResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByOrderIdUsingRelationship(Guid orderId);
        Task<QueryResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByCartIdUsingRelationship(Guid cartId);

        Task<QueryResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByCartIdWithoutAddressUsingRelationship(Guid cartId);

        Task<QueryResponseDto<IEnumerable<OrderAggregateCartViewModel>>> GetOrdersAggregateCartFilterSortPagination(
            OrderStatus filterOrderStatus,
            PaymentMethod filterPaymentMethod,
            OrdersSortBy sortBy = OrdersSortBy.Id,
            int page = 1,
            int pageSize = 10,
            OrdersSortType sortType = OrdersSortType.Ascending);

        //Can make QueryResponseDto<wrapper of primitive type int to make it reference type>
        Task<int> GetOrdersAggregateCartTotalCountAfterFiletered(OrderStatus filterOrderStatus, PaymentMethod filterPaymentMethod);
    }
}
