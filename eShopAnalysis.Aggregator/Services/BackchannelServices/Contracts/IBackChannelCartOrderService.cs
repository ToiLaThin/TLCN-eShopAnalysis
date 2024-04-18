using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelCartOrderService
    {
        //will introduce paging later
        Task<BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders(int limit = 15);
        Task<BackChannelResponseDto<IEnumerable<Guid>>> BulkApproveOrder(IEnumerable<Guid> orderIdsToApprove);

        //i do not want to use CartSummary here
        Task<BackChannelResponseDto<CartSummaryResponseDto>> AddCart(CartConfirmRequestToCartApiDto requestToCartApiDto);
        Task<BackChannelResponseDto<OrderAggregateCartResponseDto>> GetOrderAggregateCartByCartId(Guid cartId);
    }
}
