using eShopAnalysis.CartOrderAPI.Application.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Application.BackchannelServices;
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
        IBackChannelCouponSaleItemService _backChannelCouponSaleItemService;
        public CartCreateCommandHandler(IUnitOfWork uOW, IBackChannelCouponSaleItemService backChannelCouponSaleItemService)
        {
            _uOW = uOW;
            _backChannelCouponSaleItemService = backChannelCouponSaleItemService;
        }
        public async Task<CartSummary> Handle(CartCreateCommand request, CancellationToken cancellationToken)
        {
            var backChannelResponse = await _backChannelCouponSaleItemService.RetrieveCouponWithCode(request.CouponCode);            
            if (backChannelResponse.IsFailed || backChannelResponse.IsException)
            {
                _uOW.RollbackTransaction();
                string message = backChannelResponse.Error;
                return null;
            }
            
            CouponDto couponDtoRetrived = backChannelResponse.Data;

            var transaction = await _uOW.BeginTransactionAsync();
            var cartSummaryCreated = CartSummary.CreateCartSummaryFromItems(Guid.NewGuid(), request.UserId, request.CartItems);

            if (couponDtoRetrived != null)
            {
                bool appliedSuccessfully = cartSummaryCreated.ApplyCoupon(couponDtoRetrived);
                if (!appliedSuccessfully) {
                    _uOW.RollbackTransaction();
                    return null;
                }
            } else
            {
                _uOW.RollbackTransaction();
                return null;
            }

            var result = _uOW.CartRepository.Add(cartSummaryCreated);
            var cartCheckoutRequestSentDomainEvent = new CartCheckoutRequestSent(cartSummaryCreated);
            cartSummaryCreated.ToRaiseDomainEvent(cartCheckoutRequestSentDomainEvent);
            await _uOW.CommitTransactionAsync(transaction); //must use this to dispatch domain events before
            return result;
               
        }
    }
}
