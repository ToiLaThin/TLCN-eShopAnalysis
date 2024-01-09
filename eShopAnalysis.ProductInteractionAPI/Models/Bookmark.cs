using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShopAnalysis.ProductInteractionAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Bookmark
    {
        [BsonConstructor]
        public Bookmark(Guid userId, Guid productBusinessKey) {
            UserId = userId;
            ProductBusinessKey = productBusinessKey;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid BookmarkId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductBusinessKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }


    }
}
