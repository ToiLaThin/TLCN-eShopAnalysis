using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShopAnalysis.ProductInteractionAPI.Models
{
    [BsonIgnoreExtraElements]
    //rate can be 0 - 5, can be 1, 2.5 ,...
    public class Rate
    {
        [BsonConstructor]
        public Rate(Guid userId, Guid productBusinessKey, double rating)
        {
            UserId = userId;
            ProductBusinessKey = productBusinessKey;
            Rating = rating;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid RateId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductBusinessKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double Rating { get; set; }
    }
}
