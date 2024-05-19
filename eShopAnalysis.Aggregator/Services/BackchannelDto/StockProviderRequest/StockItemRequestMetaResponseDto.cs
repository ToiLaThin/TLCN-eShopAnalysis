using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto.StockProviderRequest
{
    /// <summary>
    /// response from ProviderStockApi to get stockItemRequestMeta contain productModelIds in AggregatorRead GetProductModelInfosWithStockOfProvider so we have more info to send to FE,
    /// this is StockItemRequestMetaDto, i added Response just to understand the use of it is to received from api
    /// </summary>
    public class StockItemRequestMetaResponseDto
    {
        [JsonProperty]
        public Guid ProductId { get; set; }

        [JsonProperty]
        public Guid ProductModelId { get; set; }

        [JsonProperty]
        public Guid BusinessKey { get; set; }

        [JsonProperty]
        public double UnitRequestPrice { get; set; }

        [JsonProperty]
        public int QuantityToRequestMoreFromProvider { get; set; }

        [JsonProperty]
        public int QuantityToNotify { get; set; }
    }
}
