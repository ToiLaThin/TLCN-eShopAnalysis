
namespace eShopAnalysis.StockInventory.Repository
{
    using eShopAnalysis.StockInventory.Models;
    public interface IStockInventoryRepository
    {
        Task<StockInventory> AddAsync(StockInventory stockInventory);
        StockInventory Add(StockInventory stockInventory);

        StockInventory Update(StockInventory stockInventory);

        IEnumerable<StockInventory> GetAll();

    }
}
