using eShopAnalysis.StockProviderRequestAPI.Data;
using eShopAnalysis.StockProviderRequestAPI.Models;
using MongoDB.Driver;

namespace eShopAnalysis.StockProviderRequestAPI.Repository
{
    public class ProviderRequirementRepository : IProviderRequirementRepository
    {
        private readonly MongoDbContext _context;
        public ProviderRequirementRepository(MongoDbContext context)
        {
            _context = context;
        }
        public async Task<ProviderRequirement> AddAsync(ProviderRequirement providerReqToAdd)
        {
            await _context.ProviderRequirementCollection.InsertOneAsync(providerReqToAdd);

            var filter = Builders<ProviderRequirement>.Filter.And(
                Builders<ProviderRequirement>.Filter.Eq(pR => pR.ProviderName, providerReqToAdd.ProviderName)
            );

            var findResult = await _context.ProviderRequirementCollection.FindAsync(filter);
            ProviderRequirement providerReqAdded = await findResult.SingleOrDefaultAsync();
            return providerReqAdded;
        }

        public async Task AddRangeAsync(IEnumerable<ProviderRequirement> providerRequirements)
        {
            //not gonna validate existence of these here
            await _context.ProviderRequirementCollection.InsertManyAsync(providerRequirements);
            return;
        }

        public async Task<bool> DeleteAllAsync()
        {
            FilterDefinition<ProviderRequirement> emptyFilter = Builders<ProviderRequirement>.Filter.Empty;
            var deletedResult = await _context.ProviderRequirementCollection.DeleteManyAsync(emptyFilter);
            if (deletedResult.DeletedCount <= 0) {
                return false;
            }
            return true;
        }

        public IQueryable<ProviderRequirement> GetAllAsQueryable()
        {
            IQueryable<ProviderRequirement> allQueryableProviderReqs = _context.ProviderRequirementCollection.AsQueryable();
            return allQueryableProviderReqs;
        }
        public async Task<ProviderRequirement> GetAsync(Guid providerReqId)
        {
            ProviderRequirement findResult = await _context.ProviderRequirementCollection.Find(pR => pR.ProviderRequirementId == providerReqId)
                                                                                         .SingleOrDefaultAsync();
            if (findResult == null) {
                return null;
            }
            return findResult;
        }

        public async Task<ProviderRequirement> GetByNameAsync(string providerName)
        {
            ProviderRequirement findResult = await _context.ProviderRequirementCollection.Find(pR => pR.ProviderName == providerName)
                                                                                         .SingleOrDefaultAsync();
            if (findResult == null) {
                return null;
            }
            return findResult;
        }
    }

}
