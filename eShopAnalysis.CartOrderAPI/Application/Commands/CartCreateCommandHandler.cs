using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.DomainEvents;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using MediatR;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class CartCreateCommandHandler : IRequestHandler<CartCreateCommand, CartSummary>
    {
        IUnitOfWork _uOW;
        public CartCreateCommandHandler(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }
        public async Task<CartSummary> Handle(CartCreateCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            CartSummary cartSummary = new CartSummary(Guid.NewGuid(), request.UserId); //where cart got its id,only then it can passed this id to cart item
            foreach (var cartItemToAdd in request.CartItems)
            {
                cartSummary.AddToThisItem(cartItemToAdd);
            }
            var result = _uOW.CartRepository.Add(cartSummary);
            var cartCheckoutRequestSentDomainEvent = new CartCheckoutRequestSent(cartSummary);
            cartSummary.ToRaiseDomainEvent(cartCheckoutRequestSentDomainEvent);
            await _uOW.CommitTransactionAsync(transaction);
            return result;
               
        }
    }
}
