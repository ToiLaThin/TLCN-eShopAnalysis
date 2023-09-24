namespace eShopAnalysis.ProductCatalogAPI.Domain.SeedWork
{
    public interface IDomainEventHandler<T> where T : IDomainEvent
    {
        void Handle(T @domainEvent);
    }
}
