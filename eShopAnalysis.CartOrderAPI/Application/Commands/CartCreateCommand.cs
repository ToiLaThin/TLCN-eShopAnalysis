using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using MediatR;
using System.Runtime.Serialization;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class CartCreateCommand : IRequest<CartSummary>
    {
        //[DataMember]
        public IEnumerable<CartItem> CartItems { get; private set; } 
        //TODO convert to use another model(DTO or something else)

        //[DataMember]
        public Guid UserId { get; private set; }

        public CartCreateCommand()
        {
            CartItems = new List<CartItem>();
        }

        public CartCreateCommand(IEnumerable<CartItem> cartItemsToAdd, Guid buyerId)
        {
            //is this ok
            if (buyerId == Guid.Empty || buyerId == default(Guid)) { 
                throw new ArgumentNullException(nameof(buyerId));
            }
            if (cartItemsToAdd == null || cartItemsToAdd.Count() == 0) {
                throw new ArgumentException("cart data is invalid");
            }
            this.CartItems = cartItemsToAdd;
            this.UserId = buyerId;
        }
    }
}
