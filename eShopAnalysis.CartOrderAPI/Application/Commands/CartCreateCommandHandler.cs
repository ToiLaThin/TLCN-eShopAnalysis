using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Application.BackchannelServices;
using eShopAnalysis.CartOrderAPI.Application.IntegrationEvents;
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
    public class CartCreateCommandHandler : IRequestHandler<CartCreateCommand, CartSummary>
    {
        IUnitOfWork _uOW;
        IBackChannelCouponSaleItemService _backChannelCouponSaleItemService;
        IEventBus _eventBus;
        public CartCreateCommandHandler(IUnitOfWork uOW, IBackChannelCouponSaleItemService backChannelCouponSaleItemService, IEventBus eventBus)
        {
            _uOW = uOW;
            _backChannelCouponSaleItemService = backChannelCouponSaleItemService;
            _eventBus = eventBus;
        }
        public async Task<CartSummary> Handle(CartCreateCommand request, CancellationToken cancellationToken)
        {
            UserAppliedCouponToCartIntegrationEvent toSentUserAppliedCouponToCartIntegrationEvent = null;
            var transaction = await _uOW.BeginTransactionAsync();
            var cartSummaryCreated = CartSummary.CreateCartSummaryFromItems(Guid.NewGuid(), request.UserId, request.CartItems);
            var result = _uOW.CartRepository.Add(cartSummaryCreated);

            //if request have coupon code, check if it 's exist and apply it to cart if ok, else just create cart 
            if (!request.CouponCode.IsNullOrEmpty()) {
                var backChannelResponse = await _backChannelCouponSaleItemService.RetrieveCouponWithCode(request.CouponCode);            
                if (backChannelResponse.IsFailed || backChannelResponse.IsException)
                {
                    _uOW.RollbackTransaction();
                    string message = backChannelResponse.Error;
                    return null;
                }

                var couponDtoRetrived = backChannelResponse.Data;
                if (couponDtoRetrived != null )
                {
                    bool appliedSuccessfully = cartSummaryCreated.ApplyCoupon(couponDtoRetrived);
                    if (!appliedSuccessfully) {
                        _uOW.RollbackTransaction(); return null;
                    } else {
                        toSentUserAppliedCouponToCartIntegrationEvent = new UserAppliedCouponToCartIntegrationEvent(userId: request.UserId, couponId: couponDtoRetrived.CouponId);
                    }
                } else
                {
                    _uOW.RollbackTransaction();
                    return null;
                }
            }



            //else
            var cartCheckoutRequestSentDomainEvent = new CartCheckoutRequestSentDomainEvent(cartSummaryCreated);
            cartSummaryCreated.ToRaiseDomainEvent(cartCheckoutRequestSentDomainEvent);
            await _uOW.CommitTransactionAsync(transaction); //must use this to dispatch domain events before

            //call event bus to send integration event in case there is a coupon, thus we have the to sent event, sent event after commit the transaction
            if (toSentUserAppliedCouponToCartIntegrationEvent != null)
            {
                try { _eventBus.Publish(toSentUserAppliedCouponToCartIntegrationEvent); }
                catch (Exception ex)
                {
                    throw ex;
                    _uOW.RollbackTransaction();
                    return null;
                }
            }

            return result;
        }
    }
}
