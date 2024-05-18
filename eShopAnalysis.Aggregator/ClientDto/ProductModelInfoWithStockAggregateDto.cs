namespace eShopAnalysis.Aggregator.ClientDto
{
    /// <summary>
    /// Aggregate of ProductModelInfoResponseDto & ItemStockResponseDto & unitRequestPrice & ProductModelInfoRequestMetaDto(have unitRequestPrice) by ProductModelId
    /// response to Client
    /// at provider detail: list all product model of that provider provide
    /// </summary>
    public class ProductModelInfoWithStockAggregateDto
    {
        public Guid ProductId { get; set; }

        public Guid BusinessKey { get; set; }

        public Guid ProductModelId { get; set; }

        public string ProductModelName { get; set; }

        public double Price { get; set; }

        public string ProductCoverImage { get; set; }

        public double UnitRequestPrice { get; set; }

        public int CurrentQuantity { get; set; }

        public int QuantityToRequestMoreFromProvider { get; set; }

        public int QuantityToNotify { get; set; }
    }
}
