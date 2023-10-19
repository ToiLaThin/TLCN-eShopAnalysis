namespace eShopAnalysis.CartOrderAPI.Application.Queries
{
    public interface IOrderQueries
    {
        Task<IEnumerable<OrderDraftViewModel>> GetUserDraftOrders(Guid userId);  
    }
}
