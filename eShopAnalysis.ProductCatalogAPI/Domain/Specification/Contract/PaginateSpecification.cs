namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    //can be used to create instance
    public class PaginateSpecification : IPaginateSpecification
    {
        private readonly int _page;
        private readonly int _pageSize;

        public PaginateSpecification(int page, int pageSize) {
            if (page <= 0) { throw new ArgumentOutOfRangeException("page is invalid"); }
            if (page <= 0) { throw new ArgumentOutOfRangeException("pageSize is invalid"); }
            _page = page;
            _pageSize = pageSize;
        }
        public int Page => _page;
        public int PageSize => _pageSize;
    }
}
