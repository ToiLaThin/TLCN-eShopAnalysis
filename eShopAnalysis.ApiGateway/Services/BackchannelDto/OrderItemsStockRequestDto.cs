namespace eShopAnalysis.ApiGateway.Services.BackchannelDto
{
    public record OrderItemsStockRequestDto
    {
        IEnumerable<Guid> ProductModelIds { get; }
    }
}
