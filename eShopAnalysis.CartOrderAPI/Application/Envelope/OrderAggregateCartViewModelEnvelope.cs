using eShopAnalysis.CartOrderAPI.Application.Queries;

namespace eShopAnalysis.CartOrderAPI.Application.Envelope
{
    //this record wrap list OrderAggregateCartViewModel and total count of OrderAggregateCartViewModel after filter
    //so that client know the total amount to calc the total page
    public record OrderAggregateCartViewModelListEnvelope(IEnumerable<OrderAggregateCartViewModel> OrderAggregateCartViewModels, int TotalOrdersCount);
}
