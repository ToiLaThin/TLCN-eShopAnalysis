using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class SetOrderCheckoutedOnlineCommand: IRequest<Order>
    {
        public Guid OrderId { get; }

        public PaymentMethod PaymentMethod { get; } 

        public DateTime DateCheckouted { get; }

        public SetOrderCheckoutedOnlineCommand(Guid? orderId, PaymentMethod? paymentMethod, DateTime? dateCheckouted)
        {
            //validate input from integration event
            OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
            PaymentMethod = paymentMethod ?? throw new ArgumentNullException(nameof(paymentMethod));
            DateCheckouted = dateCheckouted ?? throw new ArgumentNullException(nameof(dateCheckouted));
        }
    }
}
