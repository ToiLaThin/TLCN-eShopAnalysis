namespace eShopAnalysis.StockInventoryAPI.Dto
{
    public class StockInventoryDto
    {
        public Guid  StockInventoryId { get; set; } //convert glid to guid for dto 

        public string ProductId { get; set; }

        public string ProductModelId { get; set; }

        public string ProductBusinessKey { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
