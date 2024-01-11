using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Dto
{
    public class RewardTransactionDto
    {
        public Guid RewardTransactionId { get; set; }

        public Guid UserId { get; set; }

        public int PointTransition { get; set; }

        public DateTime DateTransition { get; set; }

        public int PointBeforeTransaction { get; set; }

        public int PointAfterTransaction { get; set; }

        public OriginJson Origin { get; set; }
    }
}
