namespace eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Mediator
{
    //refer to this https://stackoverflow.com/a/30636387
    // and eShopOnContainers at Ordering Service at Domain: https://github.dev/dotnet-architecture/eShopOnContainers
    //we can also implement a class only
    public interface IDomainEventDispatcher
    {
        void Dispatch<TEvent>(TEvent @event) where TEvent : IDomainEvent;
    }
}
