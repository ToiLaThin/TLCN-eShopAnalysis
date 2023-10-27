namespace eShopAnalysis.ApiGateway.Services.BackchannelDto
{
    public class ItemStockResponseDto
    {
        public Guid ProductModelId { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
