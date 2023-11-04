using System.Linq.Expressions;

namespace eShopAnalysis.ProductCatalogAPI.Domain.Specification
{
    //https://dotnetfalcon.com/using-the-specification-pattern-with-repository-and-unit-of-work/
    public enum OrderType
    {
        Ascending = 0,
        Descending = 1
    }

    public interface IPaginateSpecification
    {
        int Page { get; }

        int PageSize { get; }
    }


    public interface IOrderSpecification<T>
    {
        Expression<Func<T, object>> OrderBy { get; }

        OrderType OrderType { get; }
    }

    public interface IIncludeSpecification<T>
    {
        List<Expression<Func<T, object>>> Includes { get; set; }
        List<string> IncludeStrings { get; set; }
    }

    public interface IFilterSpecification<T>
    {
        List<Expression<Func<T, bool>>> Criterias { get; }

        IFilterSpecification<T> And(IFilterSpecification<T> right);

        IFilterSpecification<T> Or(IFilterSpecification<T> right);

        IFilterSpecification<T> Not(IFilterSpecification<T> original);
    }


}
