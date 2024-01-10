using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.ProductInteractionAPI.Models
{
    [BsonIgnoreExtraElements]
    //like is a mapping instance showing that user with userId liked product with product business key
    // use product business key because product may have multiple instance
    public class Like
    {
        [BsonConstructor]
        public Like(Guid userId, Guid productBusinessKey)
        {
            UserId = userId;
            ProductBusinessKey = productBusinessKey;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid LikeId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductBusinessKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }


    }
}
