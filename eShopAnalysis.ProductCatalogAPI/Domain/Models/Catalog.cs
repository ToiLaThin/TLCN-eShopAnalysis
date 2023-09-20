using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using eShopAnalysis.ProductCatalogAPI.Domain.SeedWork;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Models
{
    [BsonIgnoreExtraElements]
    public class Catalog : IAggregateRoot
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid CatalogId { get; set; }

        [BsonElement("CatalogName")]
        public string CatalogName { get; set; }

        [BsonElement("CatalogDescription")]
        public string CatalogDescription { get; set; }

        [BsonElement("CatalogImage")]
        [BsonIgnore]
        public string CatalogImage { get; set; }

        [BsonElement("CatalogSubCatalogs")]
        public List<SubCatalog> SubCatalogs { get; set; }

        public void AddSubCatalog(SubCatalog subCatalog)
        {
            SubCatalogs.Add(subCatalog);
        }

        public SubCatalog GetSubCatalog(Guid subCatalogId)
        {
            var result = SubCatalogs.Find(sc => sc.SubCatalogId == subCatalogId);
            if (result == null)
                return null;
            else 
                return result;
        }

        public SubCatalog RemoveExistingSubCatalog(Guid subCatalogId)
        {
            foreach (var subCatalogItem in SubCatalogs)
            {
                if (subCatalogItem.SubCatalogId.Equals(subCatalogId)) {
                    if (SubCatalogs.Remove(subCatalogItem) is true) {
                        return subCatalogItem;
                    };
                }
            }
            return null;
        }

        public SubCatalog UpdateExistingSubCatalog(SubCatalog newSub)
        {
            foreach (var sub in SubCatalogs)
            {
                if (sub.SubCatalogId.Equals(newSub.SubCatalogId)) { //TODO use builder to not having to delete then add again
                    if (SubCatalogs.Remove(sub) is true)
                    {
                        SubCatalogs.Add(newSub);                        
                        return newSub;
                    }
                }
            }
            return null;
        }
    }

    [BsonIgnoreExtraElements]
    public class SubCatalog
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid SubCatalogId { get; set; }

        [BsonElement("SubCatalogName")]
        public string SubCatalogName { get; set; }

        [BsonElement("SubCatalogDescription")]
        public string SubCatalogDescription { get; set; }
        
        [BsonElement("SubCatalogImage")]
        public string SubCatalogImage { get; set; }

        [BsonConstructor]
        //without this subcatlog will be create with default value 000000000000000000
        public SubCatalog()
        {
            SubCatalogId = Guid.NewGuid();
        }


    }
}
