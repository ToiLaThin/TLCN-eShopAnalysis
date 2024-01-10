using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShopAnalysis.ProductInteractionAPI.Models
{
    [BsonIgnoreExtraElements]

    public class Comment
    {
        [BsonConstructor]
        public Comment(Guid userId, Guid productBusinessKey, string commentDetail)
        {
            UserId = userId;
            ProductBusinessKey = productBusinessKey;
            CommentDetail = commentDetail;
            DateModified = DateTime.UtcNow;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CommentId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductBusinessKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string CommentDetail { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateModified{ get; set; }
    }
}
