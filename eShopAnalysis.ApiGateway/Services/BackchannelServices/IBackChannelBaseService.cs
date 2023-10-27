
using eShopAnalysis.ApiGateway.Result;

namespace eShopAnalysis.ApiGateway.Services.BackchannelServices
{
    public interface IBackChannelBaseService<S, D> where D : class where S : class
    {
        public Task<BackChannelResponseDto<D>> SendAsync(BackChannelRequestDto<S> backChannelRequestDto);
    }
}
