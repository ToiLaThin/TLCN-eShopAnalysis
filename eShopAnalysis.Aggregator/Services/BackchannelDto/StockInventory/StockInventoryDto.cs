namespace eShopAnalysis.Aggregator.Services.BackChannelDto
{
    //response from stock inventory when add stock of newly created product and the request too
    public class StockInventoryDto
    {
        public Guid  StockInventoryId { get; set; } //convert glid to guid for dto 

        public string ProductId { get; set; }

        public string ProductModelId { get; set; }

        public string ProductBusinessKey { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
