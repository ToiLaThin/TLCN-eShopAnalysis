
namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Dto.BackchannelDto
{
    //received from aggregator write controller to change reward point balance for completing order
    public class RewardTransactionForCompleteOrderingAddRequestDto
    {
        public Guid UserId { get; set; }

        public int PointTransition { get; set; }

        public double OrderPrice { get; set; }
    }
}
