using eShopAnalysis.CouponSaleItemAPI.Dto;
using Newtonsoft.Json.Linq;

namespace eShopAnalysis.CouponSaleItemAPI.Service.BackChannelService
{
    public interface IBackChannelBaseService<S, D> where D : class where S : class
    {
        public Task<BackChannelResponseDto<D>> SendAsync(BackChannelRequestDto<S> backChannelRequestDto);
    }
}
