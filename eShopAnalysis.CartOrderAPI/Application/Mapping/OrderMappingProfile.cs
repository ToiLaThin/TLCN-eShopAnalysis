using AutoMapper;
using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using System.Reflection.Metadata.Ecma335;

namespace eShopAnalysis.CartOrderAPI.Application.Mapping
{
    /// <summary>
    /// Mapping from Order to OrderAggregateCartViewModel, it only have a few different key
    /// </summary>
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile() { 
            CreateMap<Order, OrderAggregateCartViewModel>().ConstructUsing(order => new OrderAggregateCartViewModel(order));
            CreateMap<Address, AddressViewModel>().ConstructUsing(address => new AddressViewModel(address));
            CreateMap<CartItem, CartItemViewModel>().ConstructUsing(cartItem => new CartItemViewModel(
                    cartItem.ProductId,
                    cartItem.ProductModelId,
                    cartItem.BusinessKey,
                    cartItem.CartId,
                    cartItem.SaleItemId,
                    cartItem.IsOnSale,
                    cartItem.SaleType,
                    cartItem.SaleValue,
                    cartItem.Quantity,
                    cartItem.UnitPrice,
                    cartItem.FinalPrice,
                    cartItem.UnitAfterSalePrice,
                    cartItem.FinalAfterSalePrice)
            );
            CreateMap<CartSummary, CartSummaryViewModel>().ConstructUsing(cartSummary => new CartSummaryViewModel(
                cartSummary.Id,
                cartSummary.UserId,
                cartSummary.HaveCouponApplied,
                cartSummary.CouponId,
                cartSummary.HaveAnySaleItem,
                cartSummary.CouponDiscountType,
                cartSummary.CouponDiscountAmount,
                cartSummary.TotalSaleDiscountAmount,
                cartSummary.TotalPriceOriginal,
                cartSummary.TotalPriceAfterSale,
                cartSummary.TotalPriceAfterCouponApplied,
                cartSummary.TotalPriceFinal,
                cartSummary.Items.Select(cartItem => new CartItemViewModel(
                    cartItem.ProductId,
                    cartItem.ProductModelId,
                    cartItem.BusinessKey,
                    cartItem.CartId,
                    cartItem.SaleItemId,
                    cartItem.IsOnSale,
                    cartItem.SaleType,
                    cartItem.SaleValue,
                    cartItem.Quantity,
                    cartItem.UnitPrice,
                    cartItem.FinalPrice,
                    cartItem.UnitAfterSalePrice,
                    cartItem.FinalAfterSalePrice))
                )
            );
        }
    }
}
