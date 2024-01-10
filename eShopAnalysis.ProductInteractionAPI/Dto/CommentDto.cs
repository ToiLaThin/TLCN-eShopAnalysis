using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.ProductInteractionAPI.Dto
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }

        public Guid ProductBusinessKey { get; set; }

        public Guid UserId { get; set; }

        public string CommentDetail { get; set; }

        public DateTime DateModified { get; set; }

    }
}
