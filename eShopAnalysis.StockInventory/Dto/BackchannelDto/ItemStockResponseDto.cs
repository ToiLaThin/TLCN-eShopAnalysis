namespace eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto
{
    //dto to send back to api gateway aggregator
    public class ItemStockResponseDto
    {
        public Guid ProductModelId { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
