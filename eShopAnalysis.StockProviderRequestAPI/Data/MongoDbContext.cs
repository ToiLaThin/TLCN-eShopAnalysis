using eShopAnalysis.StockProviderRequestAPI.Models;
using eShopAnalysis.StockProviderRequestAPI.Utilities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eShopAnalysis.StockProviderRequestAPI.Data  
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db = null;
        private readonly IMongoClient _mongoClient;
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _mongoClient = new MongoClient(settings.Value.ConnectionString);
            if (_mongoClient is not null) {
                _db = _mongoClient.GetDatabase(settings.Value.DatabaseName);
            }
        }

        public IMongoCollection<ProviderRequirement> ProviderRequirementCollection {
            get {
                return _db.GetCollection<ProviderRequirement>("ProviderRequirementCollection");
            }
        }
        public IMongoCollection<StockRequestTransaction> StockRequestTransactionCollection {
            get {
                return _db.GetCollection<StockRequestTransaction>("StockRequestTransactionCollection");
            }
        }
    }
}
