namespace eShopAnalysis.ProductCatalogAPI.Domain.SeedWork.Mediator
{
    //gio chi can register Event va Handler tuong ung trong Program.cs
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Dispatch<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
            if (@event is null) { 
                throw new ArgumentNullException(nameof(@event));
            }

            //must use reflection and meta programming
            var allHandlersOfThisEvent = this._serviceProvider.GetServices<IDomainEventHandler<TEvent>>();
            foreach (var handler in allHandlersOfThisEvent)
            {
                handler.Handle(@event);
            }

        }
    }
}
