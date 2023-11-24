using eShopAnalysis.ProductCatalogAPI.Domain.Specification;
using Newtonsoft.Json;

namespace eShopAnalysis.ProductCatalogAPI.Application.Dto
{
    public enum SortBy
    {
        Id = 0,
        Price = 1,
        Name = 2,
    }

    public enum FilterBy
    {
        SubCatalogs = 0,
        Price = 1
    }

    public class FilterRequest {
        public FilterBy FilterBy { get; set; }

        //depend on the FilterBy, we deserialize Meta differently using JsonConvert
        //https://www.newtonsoft.com/json/help/html/deserializeobject.htm
        [JsonProperty]
        public string Meta { get; set; }

    }

    public class PriceMeta
    {
        public double FromPrice { get; set; }

        public double ToPrice { get; set; }

    }

    public class ProductLazyLoadRequestDto
    {
        public OrderType OrderType { get; set; }

        public int ProductPerPage { get; set; }

        public int PageOffset { get; set; }

        public SortBy SortBy { get; set; }

        public IEnumerable<FilterRequest> FilterRequests { get; set; }




    }
}
