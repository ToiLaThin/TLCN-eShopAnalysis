namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    /// <summary>
    /// request to stock inventory from aggregate 
    /// to add stock of newly created product
    /// response from stock inventory to aggregate 
    /// to add stock of newly created product
    /// 
    /// in aggregate write AddNewProductAndModelsStock
    /// </summary>
    public class StockInventoryDto
    {
        public Guid  StockInventoryId { get; set; } //convert glid to guid for dto 

        public string ProductId { get; set; }

        public string ProductModelId { get; set; }

        public string ProductBusinessKey { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
