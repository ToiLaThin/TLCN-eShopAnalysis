using eShopAnalysis.ProductCatalogAPI.Application.Dto;
using eShopAnalysis.ProductCatalogAPI.Domain.Models.Aggregator;
using Newtonsoft.Json;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class ProductSpecificationFactory
    {
        private IFilterSpecification<Product> _filterSpecification = null;
        private IPaginateSpecification _paginateSpecification = null;
        private IOrderSpecification<Product> _orderSpecification = null;

        private readonly ProductLazyLoadRequestDto _request;
        public ProductSpecificationFactory(ProductLazyLoadRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            _request = request;
        }

        public IFilterSpecification<Product> FilterSpecification
        {
            get
            {
                if (_request.FilterRequests.Count() <= 0) {
                    return null;
                }
                _filterSpecification = new EmptySpecification();
                //deserializing json string https://stackoverflow.com/a/44132969
                foreach (var filterReq in _request.FilterRequests)
                {
                    if (filterReq.FilterBy == FilterBy.SubCatalogs) {
                        _filterSpecification = _filterSpecification.And(new SubCatalogFilterSpecification(JsonConvert.DeserializeObject<IEnumerable<Guid>>(filterReq.Meta)));
                    }
                    else if (filterReq.FilterBy == FilterBy.Price) {
                        PriceMeta priceMeta = JsonConvert.DeserializeObject<PriceMeta>(filterReq.Meta);
                        _filterSpecification = _filterSpecification.And(new PriceFilterSpecification(priceMeta.FromPrice, priceMeta.ToPrice));
                    }
                }
                return _filterSpecification;
            }
        }


        public IPaginateSpecification PaginateSpecification
        {
            get
            {
                _paginateSpecification = new PaginateSpecification(_request.PageOffset, _request.ProductPerPage);
                return _paginateSpecification;
            }
        }

        public IOrderSpecification<Product> OrderSpecification
        {
            get
            {
                if (_request.SortBy == SortBy.Name)
                {
                    _orderSpecification = new ProductNameOrderSpecification(_request.OrderType);
                }
                else if (_request.SortBy == SortBy.Price)
                {
                    _orderSpecification = new PriceOrderSpecification(_request.OrderType);
                }
                return _orderSpecification;
            }
        }
    }
}
