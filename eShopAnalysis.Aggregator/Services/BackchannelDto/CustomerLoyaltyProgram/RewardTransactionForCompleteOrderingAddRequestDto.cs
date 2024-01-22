namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// request aggregator write to CustomerLoyaltyProgramAPI (at reward transaction controller)
    /// to change reward point balance for complete order
    /// 
    /// </summary>
    public class RewardTransactionForCompleteOrderingAddRequestDto
    {
        public Guid UserId { get; set; }

        public int PointTransition { get; set; }

        public double OrderPrice { get; set; }
    }
}
