using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate
{
    public class Order : AggregateRoot
    {
        public Order(Guid id) : base(id)
        {
        }

        private Order(Guid id, Guid businessKey, CartSummary cart): base(id)
        {
            this.Cart = cart;
            this.BusinessKey = businessKey;
        }

        public Guid CartId { get; init; }

        public CartSummary Cart { get; private set; }

        public Guid BusinessKey { get; init; }

        public string Address { get; private set; } //TODO change this to value object

        //public PaymentMethod PaymentMethod { get; set; }

        //public OrderStatus OrdersStatus { get; set; }

        public DateTime DateCreated { get; private set; }

        public DateTime? DateCheckouted { get; private set; }

        public DateTime? DateConfirmed { get; private set; }

        public DateTime? DateRefunded { get; private set; }

        public DateTime? DateCompleted { get; private set; }

        public int Revision { get; private set; }

        public static Order CreateOrderFromCart(Guid orderId, Guid businessKey, CartSummary cart)
        {
            var orderToCreate = new Order(orderId, businessKey, cart);
            orderToCreate.DateCreated = DateTime.Now;
            orderToCreate.Address = "test address";
            return orderToCreate;
        }

    }
}
