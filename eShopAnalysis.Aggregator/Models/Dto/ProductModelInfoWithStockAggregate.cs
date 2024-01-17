namespace eShopAnalysis.Aggregator.Models.Dto
{
    /// <summary>
    /// Aggregate of ProductModelInfoResponseDto & ItemStockResponseDto & unitRequestPrice & ProductModelInfoRequestMetaDto(have unitRequestPrice) by ProductModelId
    /// To return to UI at provider detail: list all product model of that provider provide
    /// </summary>
    public class ProductModelInfoWithStockAggregate
    {
        public Guid ProductId { get; set; }

        public Guid BusinessKey { get; set; }

        public Guid ProductModelId { get; set; }

        public string ProductModelName { get; set; }

        public double Price { get; set; }

        public string ProductCoverImage { get; set; }

        public double UnitRequestPrice { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
