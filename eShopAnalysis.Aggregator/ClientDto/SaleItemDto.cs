using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.ClientDto
{
    /// <summary>
    /// request from Client
    /// is duplicated in ClientDto & BackchannelDto because it is used also iin backchannel communication
    /// </summary>
    public class SaleItemDto
    {
        //because if and item model is  on sales multiple time, id, modelId, and business key might not be enough
        public Guid SaleItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductModelId { get; set; }
        public Guid BusinessKey { get; set; }

        public DiscountType DiscountType { get; set; }

        public double DiscountValue { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateEnded { get; set; }

        public Status SaleItemStatus { get; set; }

        public int RewardPointRequire { get; set; }
    }
}
