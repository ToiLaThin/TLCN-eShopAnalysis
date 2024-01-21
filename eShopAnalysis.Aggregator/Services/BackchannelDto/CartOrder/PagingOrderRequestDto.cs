using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //TODO change or replace this model
    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error

    /// <summary>
    /// request to cartOrder Backchannel 
    /// to limit to to approve order return to aggregate GetOrderToApprovedWithStock
    /// </summary>
    public class PagingOrderRequestDto
    {
        [JsonProperty]
        public int Limit { get; set; }

        [JsonConstructor]
        public PagingOrderRequestDto(int limit)
        {
            Limit = limit;
        }
    }
}
