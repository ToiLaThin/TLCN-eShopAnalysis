using eShopAnalysis.ProductInteractionAPI.Data;
using eShopAnalysis.ProductInteractionAPI.Models;
using MongoDB.Driver;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public class RateRepository : IRateRepository
    {
        private readonly MongoDbContext _context;

        public RateRepository(MongoDbContext context)
        {
            _context = context;
        }
        public async Task<Rate> AddAsync(Guid userId, Guid productBusinessKey, double rating)
        {
            Rate rateToAdd = new Rate(userId, productBusinessKey, rating);
            await _context.RateCollection.InsertOneAsync(rateToAdd);

            var filter = Builders<Rate>.Filter.And(
                Builders<Rate>.Filter.Eq(b => b.UserId, userId),
                Builders<Rate>.Filter.Eq(b => b.ProductBusinessKey, productBusinessKey)
            );

            var findResult = await _context.RateCollection.FindAsync(filter);
            Rate rateAdded = await findResult.SingleOrDefaultAsync();
            return rateAdded;
        }

        public IQueryable<Rate> GetAllAsQueryableAsync()
        {
            IQueryable<Rate> allQueryableRates = _context.RateCollection.AsQueryable();
            return allQueryableRates;
        }

        public async Task<Rate> GetAsync(Guid userId, Guid productBusinessKey)
        {
            Rate findResult = await _context.RateCollection.Find(r => r.UserId == userId && r.ProductBusinessKey == productBusinessKey).SingleOrDefaultAsync();
            if (findResult == null)
            {
                return null;
            }
            return findResult;
        }

        public async Task<Rate> GetAsync(Guid rateId)
        {
            Rate findResult = await _context.RateCollection.Find(r => r.RateId == rateId).SingleOrDefaultAsync();
            if (findResult == null)
            {
                return null;
            }
            return findResult;
        }

        public async Task<Rate> RemoveAsync(Guid userId, Guid productBusinessKey)
        {
            var filter = Builders<Rate>.Filter.And(
                            Builders<Rate>.Filter.Eq(r => r.UserId, userId),
                            Builders<Rate>.Filter.Eq(r => r.ProductBusinessKey, productBusinessKey)
                        );
            Rate deletedRate = await _context.RateCollection.FindOneAndDeleteAsync(
                r => r.UserId.Equals(userId) && r.ProductBusinessKey.Equals(productBusinessKey)
            );
            return deletedRate;
        }
    }
}
