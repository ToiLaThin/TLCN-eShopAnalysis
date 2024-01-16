namespace eShopAnalysis.StockProviderRequestAPI.Dto
{
    public class StockItemRequestMetaDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public Guid BusinessKey { get; set; }

        public double UnitRequestPrice { get; set; }

    }

    public class ProviderRequirementDto
    {
        public Guid ProviderRequirementId { get; set; }

        public string ProviderName { get; set; }

        public double MinPriceToBeAccepted { get; set; }

        public int MinQuantityToBeAccepted { get; set; }

        public Guid ProviderBusinessKey { get; set; }

        public int Revision { get; set; }

        public bool IsUsed { get; set; }

        public List<StockItemRequestMetaDto> AvailableStockItemRequestMetas { get; set; }

        public List<Guid> AvailableProviderCatalogIds { get; set; }

    }
}
