using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    //generic specification pattern
    public class BaseOrderSpecification<T> : IOrderSpecification<T>
    {
        private Expression<Func<T, object>> _orderBy;
        private OrderType _orderType;
        public BaseOrderSpecification(Expression<Func<T, object>> orderBy, OrderType orderType)
        {
            _orderBy = orderBy;
            _orderType = orderType;
        }

        public Expression<Func<T, object>> OrderBy
        {
            get { return _orderBy; }
        }

        public OrderType OrderType
        {
            get { return _orderType; }
        }
    }
}
