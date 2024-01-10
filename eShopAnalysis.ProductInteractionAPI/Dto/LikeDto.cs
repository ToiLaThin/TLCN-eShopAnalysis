using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Dto
{
    public class LikeDto
    {
        public Guid LikeId { get; set; }

        public Guid ProductBusinessKey { get; set; }

        public Guid UserId { get; set; }

        public LikeStatus Status { get; set; }
    }
}
