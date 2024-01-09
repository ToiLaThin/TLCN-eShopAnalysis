using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.ProductInteractionAPI.Dto
{
    public class BookmarkDto
    {
        public Guid BookmarkId { get; set; }

        public Guid ProductBusinessKey { get; set; }

        public Guid UserId { get; set; }
    }
}
