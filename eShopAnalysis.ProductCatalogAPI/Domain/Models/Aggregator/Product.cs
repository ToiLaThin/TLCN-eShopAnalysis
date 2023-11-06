
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Mediator;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Prototype;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Factory;
using eShopAnalysis.ProductCatalogAPI.Application.BackChannelDto;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator
{
    using eShopAnalysis.ProductCatalogAPI.Domain.Models;
    using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Factory;
    using static eShopAnalysis.ProductCatalogAPI.Domain.Models.ProductModel;

    [BsonIgnoreExtraElements]
    public class Product : ClonableObject, IAggregateRoot
    {
        //metadata
        private readonly IDomainEventDispatcher _domainEventDispatcher; 

        private readonly IDomainEventFactory _domainEventFactory; //to create events, 
        protected Product(Product sourceClone) : base(sourceClone) { 
            //implement how you want to clone
            //create a new row with the exact infomation and business key but different productId and increase in revision
        }

        //so many constructor, which one will be used
        //tested, this is still called      
        [BsonConstructor]
        public Product()
        {
            //default values
            HaveVariants = false;
            HavePricePerCublic = false;

            IsOnSale = false;
            ProductDisplaySaleValue = -1;
            ProductDisplaySaleType = DiscountType.NoDiscount;
            ProductDisplayPriceOnSale = -1;

            BusinessKey = Guid.NewGuid();
            Revision = 0;
        }
        public override ClonableObject Clone()
        {
            //just call the above clone constructor
            return new Product(this);
        }

        public Product(IDomainEventDispatcher domainEventDispatcher, IDomainEventFactory domainEventFactory)
        {
            _domainEventDispatcher = domainEventDispatcher;
            _domainEventFactory = domainEventFactory;
        }

        //private readonly 
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid SubCatalogId { get; set; }

        //in mongodb string field is required even if we use BsonIgnore, it just require us to input it, but do not serialized to save into the db
        public string SubCatalogName { get; set; }

        //in mongodb string field is required even if we use BsonIgnore, it just require us to input it, but do not serialized to save into the db        
        public string ProductCoverImage { get; set; }

        public bool IsOnSale { get; set; } //each time a product model is set on sale, this will be on

        public double ProductDisplaySaleValue { get; set; } //will be set on product largest product model sale

        public DiscountType ProductDisplaySaleType { get; set; }

        public double ProductDisplayPriceOnSale { get; set; }


        #region other unfamiliar meta info
        public bool HaveVariants { get; set; }

        //cublic la don vi do nhu kg, m, l
        public bool HavePricePerCublic { get; set; }

        //this is the number to know the version of this currrent product
        //every time we update the product, it create another row not updating existing one
        public int Revision { get; set; }

        //to know if all the revision product is refer to the same one
        [BsonRepresentation(BsonType.String)]
        public Guid BusinessKey { get; set; }
        #endregion

        public ProductInfo ProductInfo { get; set; }

        public List<ProductModel> ProductModels { get; set; }        


        //since the data modeling in mongo db is nesting, it introduce complex logic => very fit for domain driven design
        public ProductModel AddNewProductModel(ProductModel model)
        {            
            if (ProductModels.Count == 1 && HaveVariants is false) //mean that it have the default model represent the product
            {
                HaveVariants = true;
                ProductModels.Clear(); //discard the default model
                ProductModels.Add(model);
                return model;
            }

            ProductModels.Add(model);
            return model;
        }

        public void UpdateSubCatalog(Guid subCatalogId, string subCatalogName)
        {
            SubCatalogId = subCatalogId;
            SubCatalogName = subCatalogName;
        }
        

        //this product display sale is of the best sale of its models
        public Product UpdateProductToOnSale(Guid productModelId, Guid saleItemId, DiscountType discountType, double discountValue)
        {
            var updatedModel = this.ProductModels.Where(pm => pm.ProductModelId == productModelId).FirstOrDefault();
            if (updatedModel != null)
            {
                updatedModel.UpdateThisModelToOnSale(saleItemId, discountType, discountValue);

                double minPriceOnSaleOfModels = this.ProductModels.Where(pm => pm.IsOnSaleModel).Min(pm => pm.PriceOnSaleModel);
                ProductModel bestModelCurrentlyOnSale = ProductModels.Where(pm => pm.IsOnSaleModel)
                                                                     .Single(pm => pm.PriceOnSaleModel == minPriceOnSaleOfModels);
                this.IsOnSale = true;
                this.ProductDisplaySaleType = bestModelCurrentlyOnSale.SaleType;
                this.ProductDisplaySaleValue = bestModelCurrentlyOnSale.SaleValueModel;
                this.ProductDisplayPriceOnSale = bestModelCurrentlyOnSale.PriceOnSaleModel;
                return this;
            }
            return null;
        }

        public Product MarkThisAsNotOnSale()
        {
            //when no model is one sale
            IsOnSale = false;
            ProductDisplaySaleValue = -1;
            ProductDisplaySaleType = DiscountType.NoDiscount;
            ProductDisplayPriceOnSale = -1;
            return this;
        }
    }
}
