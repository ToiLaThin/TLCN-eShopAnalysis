namespace eShopAnalysis.ApiGateway.Services.BackchannelDto
{
    //since backchannelService always read from body
    //this is the request from apigateway to get stock of models in an order
    public record OrderItemsStockRequestDto
    {
        public IEnumerable<Guid> ProductModelIds { get; set; }
    }
}
