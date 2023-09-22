using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator
{
    [BsonIgnoreExtraElements]
    public class Product : IAggregateRoot
    {
        //metadata
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        [BsonIgnore]
        public Guid SubCatalogId { get; set; }

        //[BsonIgnore]
        ////in mongodb string field is required
        //public string SubCatalogName { get; set; }

        //[BsonIgnore]
        ////in mongodb string field is required
        //public string ProductCoverImage { get; set; }

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

        //[BsonIgnore]
        //public ProductInfo ProductInfo { get; set; }

        public List<ProductModel> ProductModels { get; set; }
        public Product() {
            IsOnSale = false;
            HaveVariants = false;
        }


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






    }
}
