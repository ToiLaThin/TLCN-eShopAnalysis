
using eShopAnalysis.CartOrderAPI.Application.Result;

namespace eShopAnalysis.CartOrderAPI.Application.BackchannelServices
{
    public interface IBackChannelBaseService<S, D> where D : class where S : class
    {
        public Task<BackChannelResponseDto<D>> SendAsync(BackChannelRequestDto<S> backChannelRequestDto);
    }
}
