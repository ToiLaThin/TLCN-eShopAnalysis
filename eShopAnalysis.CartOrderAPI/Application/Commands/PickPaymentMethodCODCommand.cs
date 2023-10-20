using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using MediatR;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    //TODO see if this is the right data to return 
    public class PickPaymentMethodCODCommand: IRequest<OrderViewModel>
    {
        public Guid OrderId { get; set; }

        public PickPaymentMethodCODCommand(Guid orderId)
        {
            this.OrderId = orderId;
        }

        public PickPaymentMethodCODCommand(string orderId)
        {
            this.OrderId = Guid.Parse(orderId);
        }
    }
}
