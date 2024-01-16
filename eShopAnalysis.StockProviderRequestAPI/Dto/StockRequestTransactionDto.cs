using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.StockProviderRequestAPI.Dto
{
    public class StockItemRequestDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductModelId { get; set; }

        public Guid BusinessKey { get; set; }

        public int ItemQuantity { get; set; }

        public double UnitRequestPrice { get; set; } //different from unitPrce of product

        public double TotalItemRequestPrice { get; set; }
    }

    public class StockRequestTransactionDto
    {
        public Guid StockRequestTransactionId { get; set; }

        public Guid ProviderRequirementId { get; set; }

        public Guid ProviderBusinessKey { get; set; }

        public double TotalTransactionPrice { get; set; }

        public int TotalQuantity { get; set; }

        public DateTime DateCreated { get; set; }

        List<StockItemRequestDto> StockItemRequests { get; set; }
    }
}
