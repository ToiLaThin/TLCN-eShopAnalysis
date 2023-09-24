
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Mediator;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Prototype;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.FactoryMethod;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator
{
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
        public Product()
        {
            IsOnSale = true;
            HaveVariants = false;
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

        [BsonIgnore]
        //in mongodb string field is required even if we use BsonIgnore, it just require us to input it, but do not serialized to save into the db        
        public string ProductCoverImage { get; set; }

        public bool IsOnSale { get; set; } //each time a product model is set on sale, this will be on

        [BsonIgnore]
        public double SalePercent{ get; set; } //will be set on product largest product model sale

        [BsonIgnore]
        public double PriceOnSale{ get; set; }


        #region other unfamiliar meta info
        public bool HaveVariants { get; set; }

        //cublic la don vi do nhu kg, m, l
        [BsonIgnore]
        public bool HavePricePerCublic { get; set; }

        //this is the number to know the version of this currrent product
        //every time we update the product, it create another row not updating existing one
        [BsonIgnore]
        public int Revision { get; set; }

        //to know if all the revision product is refer to the same one
        [BsonIgnore]
        public Guid BusinessKey { get; set; }
        #endregion

        [BsonIgnore]
        public ProductInfo ProductInfo { get; set; }

        public List<ProductModel> ProductModels { get; set; }        


        //since the data modeling in mongo db is nesting, it introduce complex logic => very fit for domain driven design
        public ProductModel AddNewProductModel(ProductModel model)
        {
            if (ProductModels.Count == 0)
            {
                var defaultProductModel = new ProductModel()
                {
                    ProductModelId = Guid.NewGuid(),
                    Price = 5
                };
                ProductModels.Add(defaultProductModel); //this product models only have one , which is the product itself
                return defaultProductModel;
            }
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

        
    }
}
