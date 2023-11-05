using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.DomainEvents;
using eShopAnalysis.CartOrderAPI.Infrastructure;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using eShopAnalysis.EventBus.Abstraction;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace eShopAnalysis.CartOrderAPI.Application.Commands
{
    public class CartCreateCommandHandler : IRequestHandler<CartCreateCommand, CommandHandlerResponseDto<CartSummary>>
    {
        IUnitOfWork _uOW;
        IEventBus _eventBus;
        public CartCreateCommandHandler(IUnitOfWork uOW, IEventBus eventBus)
        {
            _uOW = uOW;
            _eventBus = eventBus;
        }
        public async Task<CommandHandlerResponseDto<CartSummary>> Handle(CartCreateCommand request, CancellationToken cancellationToken)
        {
            UserAppliedCouponToCartIntegrationEvent toSentUserAppliedCouponToCartIntegrationEvent = null;
            var transaction = await _uOW.BeginTransactionAsync();
            var cartSummaryCreated = CartSummary.CreateCartSummaryFromItems(Guid.NewGuid(), request.UserId, request.CartItems);
            var result = await _uOW.CartRepository.AddAsync(cartSummaryCreated);

            //if request have coupon code, check if it 's exist and apply it to cart if ok, else just create cart 
            if (request.Coupon != null) {
                bool appliedSuccessfully = cartSummaryCreated.ApplyCoupon(request.Coupon);
                if (!appliedSuccessfully) {
                    _uOW.RollbackTransaction(); 
                    return CommandHandlerResponseDto<CartSummary>.Failure("Retrived coupon but cannot applied"); ;
                } 
                toSentUserAppliedCouponToCartIntegrationEvent = new UserAppliedCouponToCartIntegrationEvent(userId: request.UserId, couponId: request.Coupon.CouponId);
            }

            var cartCheckoutRequestSentDomainEvent = new CartCheckoutRequestSentDomainEvent(cartSummaryCreated);
            cartSummaryCreated.ToRaiseDomainEvent(cartCheckoutRequestSentDomainEvent);
            await _uOW.CommitTransactionAsync(transaction); //must use this to dispatch domain events before

            //call event bus to send integration event in case there is a coupon, thus we have the to sent event, sent event after commit the transaction
            if (toSentUserAppliedCouponToCartIntegrationEvent != null) {
                try { 
                    _eventBus.Publish(toSentUserAppliedCouponToCartIntegrationEvent); 
                }
                catch (Exception ex) {
                    throw ex;
                }
            }

            return CommandHandlerResponseDto<CartSummary>.Success(result);
        }
    }
}
