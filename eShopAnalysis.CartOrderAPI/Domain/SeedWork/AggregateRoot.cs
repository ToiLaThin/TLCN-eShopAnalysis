namespace eShopAnalysis.CartOrderAPI.Domain.SeedWork
{
    public abstract class AggregateRoot : Entity
    {
        protected AggregateRoot(Guid id) : base(id)
        {
        }
    }
}
