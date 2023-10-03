using Redis.OM.Modeling;
namespace eShopAnalysis.StockInventory.Models
{
    [Document(StorageType = StorageType.Hash, Prefixes = new[] { $"{nameof(StockInventory)}" })]
    //[Document(StorageType = StorageType.Hash)]
    public class StockInventory
    {
        [RedisIdField]
        public Ulid? StockInventoryId { get; set; }

        [Indexed(Sortable = false, CaseSensitive = true, Aggregatable = true)] //will be group and search so need index
        public string ProductId { get; set; } //must be convert from dto guid => string

        [Indexed(Sortable = false, CaseSensitive = true)] 
        //[Searchable(Weight = 1)] can only have one attribute applied to it
        public string ProductModelId { get; set; } //must be convert from dto guid => string

        [Indexed(Sortable = false, CaseSensitive = true, Aggregatable = true)] //will be group and search so need index 
        public string ProductBusinessKey { get; set; } //must be convert from dto guid => string

        [Indexed(Sortable = true, Aggregatable = true)] //can be sum min, max so need index
        public int CurrentQuantity { get; set; }
    }
}
