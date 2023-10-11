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

    public bool IsOnSaleModel { get; set; }

    public double SaleValueModel { get; set; }

    public DiscountType SaleType { get; set; }

    public double PriceOnSaleModel { get; set; }

    [BsonConstructor]
    public ProductModel()
    {
        ProductModelId = Guid.NewGuid();
        //set default value
        IsOnSaleModel = false;
        SaleType = DiscountType.NoDiscount;// them 1 discount Type là none cả trên front end và backend
        PriceOnSaleModel = -1;
        SaleValueModel = -1;

        PricePerCublicValue = -1;
        CublicValue = -1;
        CublicType = CublicType.N;
    }

    public enum DiscountType
    {
        ByValue,
        ByPercent,
        NoDiscount
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

    public ProductModel ClearSaleOfThisModel()
    {
        IsOnSaleModel = false;
        SaleType = DiscountType.NoDiscount;// them 1 discount Type là none cả trên front end và backend
        PriceOnSaleModel = -1;
        SaleValueModel = -1;
        return this;
    }
}
