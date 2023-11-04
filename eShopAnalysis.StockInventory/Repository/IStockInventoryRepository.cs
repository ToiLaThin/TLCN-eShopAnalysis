
namespace eShopAnalysis.StockInventory.Repository
{
    using eShopAnalysis.StockInventory.Models;
    public interface IStockInventoryRepository
    {
        Task<StockInventory> AddAsync(StockInventory stockInventory);
        StockInventory Add(StockInventory stockInventory);

        IQueryable<StockInventory> GetAsQueryable();

        public Task UpdateAsync(StockInventory stockInventory);

        public void Update(StockInventory stockInventory);
    }
}
