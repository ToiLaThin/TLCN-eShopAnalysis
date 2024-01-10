using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.ProductInteractionAPI.Dto
{
    public class RateDto
    {
        public Guid RateId { get; set; }

        public Guid ProductBusinessKey { get; set; }

        public Guid UserId { get; set; }

        public double Rating { get; set; }
    }
}
