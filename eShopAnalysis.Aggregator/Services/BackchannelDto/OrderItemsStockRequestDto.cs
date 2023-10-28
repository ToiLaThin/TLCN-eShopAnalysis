using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    public record OrderItemsStockRequestDto
    {
        //json prop is required for the model to be serialized or deserialized correctly
        //if not , there will be error
        [JsonProperty]
        IEnumerable<Guid> ProductModelIds { get; }

        [JsonConstructor]
        public OrderItemsStockRequestDto(IEnumerable<Guid> productModelIds)
        {
            this.ProductModelIds = productModelIds;
        }
    }
}
