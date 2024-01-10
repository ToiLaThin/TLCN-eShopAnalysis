using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.ProductInteractionAPI.Models
{
    public enum LikeStatus
    {
        //a like is of three type:
        //+ Liked: means the Like represent user liked the product
        //+ Neutral: means that the Like represent user unlike: not liked or disliked
        //+ Disliked: means that the Like represent user dislike the product
        Liked = 0,
        Neutral = 1,
        Disliked = 2
    }

    [BsonIgnoreExtraElements]
    //like is a mapping instance showing that user with userId liked product with product business key
    // use product business key because product may have multiple instance
    public class Like
    {
        [BsonConstructor]
        public Like(Guid userId, Guid productBusinessKey, LikeStatus status)
        {
            UserId = userId;
            ProductBusinessKey = productBusinessKey;
            Status = status;
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid LikeId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductBusinessKey { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public LikeStatus Status { get; set; }


    }
}
