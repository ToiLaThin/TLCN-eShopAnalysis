using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.SeedWork;

namespace eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate
{
    public enum OrderStatus
    {
        CreatedDraft = 0,
        CustomerInfoConfirmed = 1,
        Checkouted = 2,
        StockConfirmed = 3,
        Refunded = 4,
        Cancelled = 5,
        Completed = 6
    }

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

        public Address? Address { get; private set; }

        public string? PhoneNumber { get; private set; }

        //public PaymentMethod PaymentMethod { get; set; }

        public OrderStatus OrdersStatus { get; set; }

        public DateTime DateCreatedDraft { get; private set; }

        public DateTime? DateCustomerInfoConfirmed { get; private set; }

        public DateTime? DateCheckouted { get; private set; }

        public DateTime? DateStockConfirmed { get; private set; }

        public DateTime? DateRefunded { get; private set; }

        public DateTime? DateCancelled { get; private set; }

        public DateTime? DateCompleted { get; private set; }

        public int Revision { get; private set; }
        private bool MarkCreatedDraft()
        {
            //TODO please review this
            //this means you can not use this func if order is already completed or cancelled,..
            //and this method is only callable in the CreateOrderFromCart factory method
            if (this.OrdersStatus != null || this.OrdersStatus != OrderStatus.CustomerInfoConfirmed)
                return false;
            this.DateCreatedDraft = DateTime.Now;
            this.OrdersStatus = OrderStatus.CreatedDraft;
            //check logic to see if we can created draft
            return true; 
        }
        public static Order CreateOrderFromCart(Guid orderId, Guid businessKey, CartSummary cart)
        {
            var orderToCreate = new Order(orderId, businessKey, cart);
            orderToCreate.MarkCreatedDraft();
            return orderToCreate;
        }

        
        //make sure we can only call this when the order status is created draft
        private bool MarkCustomerInfoConfirmed()
        {
            if (this.OrdersStatus != OrderStatus.CreatedDraft && this.DateCustomerInfoConfirmed != null)
                return false;
            this.DateCustomerInfoConfirmed = DateTime.Now;
            this.OrdersStatus = OrderStatus.CustomerInfoConfirmed;
            return true;
        }
        public bool UpdateCustomerInfoOnThisOrder(Address address, string phoneNumber)
        {
            if (address == null) {
                throw new ArgumentNullException("address is required");
            }
            if (phoneNumber == null) {
                throw new ArgumentNullException("phone number is required");
            }
            //we validated on the upper layer
            bool canDoThis = this.MarkCustomerInfoConfirmed();
            if (canDoThis) {
                this.Address = address; //value object asignment
                this.PhoneNumber = phoneNumber;
                return true;
            }
            return false;
        }

    }
}
