using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShopAnalysis.StockProviderRequestAPI.Models
{
    public class StockItemRequest
    {
        public StockItemRequest() { }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductModelId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid BusinessKey { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int ItemQuantity { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double UnitRequestPrice { get; set; } //different from unitPrce of product

        [BsonRepresentation(BsonType.Double)]
        public double TotalItemRequestPrice { get; set; } 
    }

    [BsonIgnoreExtraElements]
    public class StockRequestTransaction
    {
        [BsonConstructor]
        public StockRequestTransaction()
        {
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid StockRequestTransactionId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProviderRequirementId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProviderBusinessKey { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double TotalTransactionPrice { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TotalQuantity { get; set; }

        [BsonRepresentation(BsonType.DateTime)]
        public DateTime DateCreated{ get; set; }

        public List<StockItemRequest> StockItemRequests { get; set; }
    }
}
