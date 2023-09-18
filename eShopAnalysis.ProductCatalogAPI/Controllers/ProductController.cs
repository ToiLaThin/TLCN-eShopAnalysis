using eShopAnalysis.ProductCatalogAPI.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace eShopAnalysis.ProductCatalogAPI.Controllers
{
    [Route("api/ProductAPI")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MongoDbContext _db;
        public ProductController(MongoDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public object GetAll()
        {
            _db.ProductCollection.InsertOne(new Models.Product()
            {
                //ProductId = Guid.NewGuid(), //have guiid generator register automatically, refer to this:, but you can specify it also
                //https://mongodb.github.io/mongo-csharp-driver/2.0/reference/bson/mapping/
                Name = "nah",
                Description = "Description",
            });
            var result = _db.ProductCollection.AsQueryable().ToList();

            return result;
        }
    }
}
