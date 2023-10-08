using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Models;

public enum CublicType
{
    M, //khoi luowng
    V, //the tich
    S, //dien tich
    N //do not have cubic

};

public class ProductModel
{
    //TODO remove some bson ignore
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid ProductModelId { get; set; }

    [BsonIgnore]
    public IEnumerable<string> ProductModelThumbnails { get; set; }

    [BsonElement("CublicType")]
    [BsonRepresentation(BsonType.Int32)]
    public CublicType CublicType { get; set; }

    [BsonElement("CublicValue")]
    [BsonRepresentation(BsonType.Double)]
    public double CublicValue { get; set; } //2kg, 3m

    [BsonElement("PricePerCublicValue")]
    [BsonRepresentation(BsonType.Double)]
    public double PricePerCublicValue { get; set; }
    public double CublicPrice { get; set; } //price per cublic like 1000d / 1kg

    public double Price { get; set; }

    [BsonIgnore]
    public bool IsOnSaleModel { get; set; }

    [BsonIgnore]
    public double SaleValueModel { get; set; }

    [BsonIgnore]
    public DiscountType SaleType { get; set; }

    [BsonIgnore]
    public double PriceOnSaleModel { get; set; }

    [BsonConstructor]
    public ProductModel()
    {
        ProductModelId = Guid.NewGuid();
    }

    public enum DiscountType
    {
        ByValue,
        ByPercent
    }

    public ProductModel UpdateThisModelToOnSale(DiscountType discountType, double discountValue)
    {
        this.IsOnSaleModel = true;
        this.SaleType = discountType;
        this.SaleValueModel = discountValue;
        switch (discountType)
        {
            case DiscountType.ByPercent:
                this.PriceOnSaleModel = this.Price - (discountValue * this.Price);
                break;
            case DiscountType.ByValue:
                this.PriceOnSaleModel = this.Price - discountValue;
                break;

        }
        return this;
    }
}
