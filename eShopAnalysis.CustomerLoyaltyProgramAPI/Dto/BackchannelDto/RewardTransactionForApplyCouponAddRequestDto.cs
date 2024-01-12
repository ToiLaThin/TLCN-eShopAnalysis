using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Dto.BackchannelDto
{
    //received from aggregator write controller to change reward point balance for apply coupon
    public class RewardTransactionForApplyCouponAddRequestDto
    {
        public Guid UserId { get; set; }

        public CouponDiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; }

        public int PointTransition { get; set; }

    }
}
