namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //sent from aggregator write controller to change reward point balance for complete order
    //to customer loyalty program api
    //in reward transaction controller
    public class RewardTransactionForCompleteOrderingAddRequestDto
    {
        public Guid UserId { get; set; }

        public int PointTransition { get; set; }

        public double OrderPrice { get; set; }
    }
}
