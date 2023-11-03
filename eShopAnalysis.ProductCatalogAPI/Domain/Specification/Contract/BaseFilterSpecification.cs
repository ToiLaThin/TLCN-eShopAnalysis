using AutoMapper.Execution;
using System.Linq.Expressions;
using ZstdSharp.Unsafe;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _parameter;
        internal ParameterReplacer(ParameterExpression parameter)
        {
            _parameter = parameter;
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }
    }

    public class AndFilterSpecification<T> : BaseFilterSpecification<T>
    {
        private readonly IFilterSpecification<T> _left;

        private readonly IFilterSpecification<T> _right;

        public AndFilterSpecification(IFilterSpecification<T> left, IFilterSpecification<T> right) : base()
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> Criteria
        {
            get
            {
                var leftCriteria = _left.Criteria;
                var rightCriteria = _right.Criteria;

                var paramExpr = Expression.Parameter(typeof(T));
                BinaryExpression exprBody = Expression.AndAlso(leftCriteria.Body, rightCriteria.Body);
                exprBody = (BinaryExpression)new ParameterReplacer(paramExpr).Visit(exprBody);
                return Expression.Lambda<Func<T, bool>>(exprBody, paramExpr);
            }
        }

    }

    //composite filter specification
    //https://thecodeblogger.com/2021/07/02/net-composite-specifications-using-ef-core/
    public abstract class BaseFilterSpecification<T> : IFilterSpecification<T>
    {
        public virtual Expression<Func<T, bool>> Criteria { get; }

        protected BaseFilterSpecification() { }
        public BaseFilterSpecification(Expression<Func<T, bool>> filter)
        {
            Criteria = filter;
        }

        public IFilterSpecification<T> And(IFilterSpecification<T> right)
        {
            return new AndFilterSpecification<T>(this, right);
        }

        public IFilterSpecification<T> Not(IFilterSpecification<T> original)
        {
            throw new NotImplementedException();
        }

        public IFilterSpecification<T> Or(IFilterSpecification<T> right)
        {
            throw new NotImplementedException();
        }
    }
}
