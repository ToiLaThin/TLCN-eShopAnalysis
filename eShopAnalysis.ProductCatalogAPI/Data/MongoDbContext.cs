using eShopAnalysis.ProductCatalogAPI.Models;
using eShopAnalysis.ProductCatalogAPI.Utilities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Data  
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _db = null;
        public MongoDbContext(IOptions<MongoDbSettings> settings) 
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            if (mongoClient is not null)
            {
                _db = mongoClient.GetDatabase(settings.Value.DatabaseName);                

            }
        }

        public IMongoCollection<Product> ProductCollection
        {
            get
            {
                return _db.GetCollection<Product>("Products");
            }
        }
    }
}
