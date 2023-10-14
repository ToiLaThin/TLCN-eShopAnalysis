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
            var cartSummaryCreated = CartSummary.CreateCartSummaryFromItems(Guid.NewGuid(), request.UserId, request.CartItems);
            var result = _uOW.CartRepository.Add(cartSummaryCreated);
            var cartCheckoutRequestSentDomainEvent = new CartCheckoutRequestSent(cartSummaryCreated);
            cartSummaryCreated.ToRaiseDomainEvent(cartCheckoutRequestSentDomainEvent);
            await _uOW.CommitTransactionAsync(transaction);
            return result;
               
        }
    }
}
