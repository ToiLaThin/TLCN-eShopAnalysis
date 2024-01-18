namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    public class StockItemRequestDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public Guid BusinessKey { get; set; }

        public int ItemQuantity { get; set; }

        public double UnitRequestPrice { get; set; } //different from unitPrce of product

        public double TotalItemRequestPrice { get; set; }
    }

    /// <summary>
    /// sent to StockProviderRequest to add new stock request transaction through backchannel
    /// </summary>
    public class StockRequestTransactionDto
    {
        public Guid StockRequestTransactionId { get; set; }

        public Guid ProviderRequirementId { get; set; }

        public Guid ProviderBusinessKey { get; set; }

        public double TotalTransactionPrice { get; set; }

        public int TotalQuantity { get; set; }

        public DateTime DateCreated { get; set; }

        public List<StockItemRequestDto> StockItemRequests { get; set; }
    }
}
