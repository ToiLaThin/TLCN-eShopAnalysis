using eShopAnalysis.StockProviderRequestAPI.Data;
using eShopAnalysis.StockProviderRequestAPI.Models;
using MongoDB.Driver;

namespace eShopAnalysis.StockProviderRequestAPI.Repository
{
    public class StockRequestTransactionRepository : IStockRequestTransactionRepository
    {
        private readonly MongoDbContext _context;
        public StockRequestTransactionRepository(MongoDbContext context)
        {
            _context = context;
        }
        public async Task<StockRequestTransaction> AddAsync(StockRequestTransaction stockReqTransaction)
        {
            await _context.StockRequestTransactionCollection.InsertOneAsync(stockReqTransaction);

            var filter = Builders<StockRequestTransaction>.Filter.And(
                Builders<StockRequestTransaction>.Filter.Eq(stockReqTrans => stockReqTrans.StockRequestTransactionId, stockReqTransaction.StockRequestTransactionId)
            );

            var findResult = await _context.StockRequestTransactionCollection.FindAsync(filter);
            StockRequestTransaction providerReqAdded = await findResult.SingleOrDefaultAsync();
            return providerReqAdded;
        }

        public async Task<bool> DeleteAllAsync()
        {
            FilterDefinition<StockRequestTransaction> emptyFilter = Builders<StockRequestTransaction>.Filter.Empty;
            var deletedResult = await _context.StockRequestTransactionCollection.DeleteManyAsync(emptyFilter);
            if (deletedResult.DeletedCount <= 0)
            {
                return false;
            }
            return true;
        }

        public IQueryable<StockRequestTransaction> GetAllAsQueryable()
        {
            IQueryable<StockRequestTransaction> allStockTransactionReqs = _context.StockRequestTransactionCollection.AsQueryable();
            return allStockTransactionReqs;
        }

        public async Task<StockRequestTransaction> GetAsync(Guid stockReqTransId)
        {
            StockRequestTransaction findResult = await _context.StockRequestTransactionCollection.Find(stockReqTrans => stockReqTrans.StockRequestTransactionId == stockReqTransId)
                                                                                                     .SingleOrDefaultAsync();
            if (findResult == null)
            {
                return null;
            }
            return findResult;
        }
    }
}
