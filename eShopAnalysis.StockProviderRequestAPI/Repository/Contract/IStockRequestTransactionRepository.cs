using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Repository
{
    public interface IStockRequestTransactionRepository
    {
        IQueryable<StockRequestTransaction> GetAllAsQueryable();

        Task<StockRequestTransaction> GetAsync(Guid stockReqTransId);

        Task<StockRequestTransaction> AddAsync(StockRequestTransaction stockReqTransaction);

        Task<bool> DeleteAllAsync();
    }
}
