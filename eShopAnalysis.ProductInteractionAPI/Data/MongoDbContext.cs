using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Utilities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace eShopAnalysis.ProductInteractionAPI.Data  
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

        public IMongoCollection<Bookmark> BookmarkCollection {
            get {
                return _db.GetCollection<Bookmark>("BookmarkCollection");
            }
        }
    }
}
