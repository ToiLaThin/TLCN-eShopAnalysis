using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.Services.BackchannelDto
{
    //TODO change or replace this model
    //json prop is required for the model to be serialized or deserialized correctly
    //if not , there will be error
    public class PagingOrderRequestDto
    {
        [JsonProperty]
        public int Limit { get; set; }

        [JsonConstructor]
        public PagingOrderRequestDto(int limit)
        {
            this.Limit = limit;
        }
    }
}
