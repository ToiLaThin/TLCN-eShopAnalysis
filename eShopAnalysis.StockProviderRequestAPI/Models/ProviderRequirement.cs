using eShopAnalysis.StockProviderRequestAPI.Utilities.Prototype;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShopAnalysis.StockProviderRequestAPI.Models
{

    public class StockItemRequestMeta
    {
        //require _id field or not ?

        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid ProductModelId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid BusinessKey { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double UnitRequestPrice { get; set; }
        
        [BsonRepresentation(BsonType.Int32)]
        public int QuantityToRequestMoreFromProvider { get; set; } //should be positive
        
        [BsonRepresentation(BsonType.Int32)]
        public int QuantityToNotify { get; set; }  //this should > QuantityToRequestMoreFromProvider, positive

        public StockItemRequestMeta() { }
    }

    [BsonIgnoreExtraElements]
    public class ProviderRequirement: ClonableObject
    {
        [BsonConstructor]
        public ProviderRequirement() {
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ProviderRequirementId { get; set; }

        [BsonRepresentation(BsonType.String)]
        public string ProviderName { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public double MinPriceToBeAccepted { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int MinQuantityToBeAccepted { get; set; }


        [BsonRepresentation(BsonType.String)]
        public Guid ProviderBusinessKey { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Revision { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsUsed { get; set; }

        public List<StockItemRequestMeta> AvailableStockItemRequestMetas { get; set; }

        //review this carefully later
        [BsonRepresentation(BsonType.String)]
        public List<Guid> AvailableProviderCatalogIds { get; set; }

        public override ClonableObject Clone()
        {
            throw new NotImplementedException();
        }
    }
}
