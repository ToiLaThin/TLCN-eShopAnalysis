using eShopAnalysis.StockInventory.Data;

namespace eShopAnalysis.StockInventory.Repository
{
    using eShopAnalysis.StockInventory.Models;
    using System.Threading.Tasks;

    public class StockInventoryRepository : IStockInventoryRepository
    {
        private readonly RedisDbContext _redisContext;
        public StockInventoryRepository(RedisDbContext redisContext)
        {
            _redisContext = redisContext;
        }
        public StockInventory Add(StockInventory stockInventory)
        {
            stockInventory.StockInventoryId = Ulid.NewUlid();
            string insertedStockId = _redisContext.StockInventoryCollection.Insert(stockInventory);
            if (!string.IsNullOrEmpty(insertedStockId))
            {
                var insertedStock = _redisContext.StockInventoryCollection.FindById(insertedStockId);
                if (insertedStock != null)
                {
                    return insertedStock;
                }
            }
            return null;
        }

        public async Task<StockInventory> AddAsync(StockInventory stockInventory)
        {
            string insertedStockId = await _redisContext.StockInventoryCollection.InsertAsync(stockInventory);
            if (!string.IsNullOrEmpty(insertedStockId))
            {
                var insertedStock = await _redisContext.StockInventoryCollection.FindByIdAsync(insertedStockId);
                if (insertedStock != null)
                {
                    return insertedStock;
                }
            }
            return null;
        }

        public IEnumerable<StockInventory> GetAll()
        {
            try
            {
                var result = _redisContext.StockInventoryCollection.Select(x => x).ToList();
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        //must find then update the returned stockInventory
        public async Task UpdateAsync(StockInventory stockInventory)
        {
            await _redisContext.StockInventoryCollection.UpdateAsync(stockInventory);
        }

        public void Update(StockInventory stockInventory)
        {
            _redisContext.StockInventoryCollection.Update(stockInventory);
        }
    }
}
