namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// request Client to Aggregate write
    /// in AddStockReqTransAndIncreaseStockItems to add stock request trans & increase stock item
    /// request & response Aggregate writeto StockProviderRequest Backchannel to add stock request transaction
    /// to add new stock request transaction through backchannel
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

        public class StockItemRequestDto
        {
            public Guid ProductId { get; set; }

            public Guid ProductModelId { get; set; }

            public Guid BusinessKey { get; set; }

            public int ItemQuantity { get; set; }

            public int CurrentItemQuantityInStockBeforeThisStockRequest { get; set; }

            public int DistanceToReachNotifyQuantityLevelBeforeThisStockRequest { get; set; } //positive, less than DistanceToReachOrderMoreQuantityLevel

            public int DistanceToReachOrderMoreQuantityLevelBeforeThisStockRequest { get; set; } //positive, more than DistanceToReachNotifyQuantityLevel

            public double UnitRequestPrice { get; set; } //different from unitPrce of product

            public double TotalItemRequestPrice { get; set; }
        }
    }
}
