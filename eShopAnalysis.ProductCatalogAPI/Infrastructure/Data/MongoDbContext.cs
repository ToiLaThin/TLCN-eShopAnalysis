using eShopAnalysis.ProductCatalogAPI.Domain.Models;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eShopAnalysis.ProductCatalogAPI.Infrastructure.Data  
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db = null;
        private readonly IMongoClient _mongoClient;
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            _mongoClient = new MongoClient(settings.Value.ConnectionString);
            if (_mongoClient is not null)
            {
                _db = _mongoClient.GetDatabase(settings.Value.DatabaseName);
                //learn later
                //var indexKeysDefinition = Builders<Product>.IndexKeys.Combine(
                //        Builders<Product>.IndexKeys.Ascending(p => p.ProductId),
                //        Builders<Product>.IndexKeys.Ascending(p => p.ProductModels.AsQueryable().Select(pm => pm.ProductModelId)

                //));
                //var productCollectionIndexModel = new CreateIndexModel<Product>(indexKeysDefinition);
                //_db.GetCollection<Product>("ProductCollection").Indexes.CreateOne(productCollectionIndexModel);
            }
        }
        public IClientSessionHandle GetClientSession()
        {
            var clientSession = _mongoClient.StartSession();
            return clientSession;
        }

        public IMongoCollection<Catalog> CatalogCollection
        {
            get
            {
                return _db.GetCollection<Catalog>("CatalogCollection");
            }
        }

        public IMongoCollection<Product> ProductCollection {
            get
            {
                return _db.GetCollection<Product>("ProductCollection");
            }
        }
    }
}
