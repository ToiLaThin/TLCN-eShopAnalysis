using Newtonsoft.Json;

namespace eShopAnalysis.Aggregator.ClientDto
{
    /// <summary>
    /// request from Client 
    /// to approve order (change status) & (decrease stock of order item)
    /// </summary>
    public class OrderApprovedAggregateDto
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public double OrderPrice { get; set; }

        public IEnumerable<StockDecreaseRequestDto> OrderItemsStockToChange { get; set; }


        /// <summary>
        /// This class is OrderApprovedAggregateDto property, there is a model with same structure send to StockInventory back channel to decrease stock quantity of product models
        /// </summary>
        public class StockDecreaseRequestDto
        {
            [JsonProperty]
            public Guid ProductModelId { get; set; }

            [JsonProperty]
            public int QuantityToDecrease { get; set; }

            [JsonConstructor]
            public StockDecreaseRequestDto(Guid productModelId, int quantityToDecrease)
            {
                ProductModelId = productModelId;
                QuantityToDecrease = quantityToDecrease;
            }

            public StockDecreaseRequestDto() { }
        }
    }
}
