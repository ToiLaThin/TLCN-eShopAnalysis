using eShopAnalysis.ProductCatalogAPI.Application.Result;

namespace eShopAnalysis.ProductCatalogAPI.Application.BackchannelServices
{
    public interface IBackChannelBaseService<S, D> where D : class where S : class
    {
        public Task<BackChannelResponseDto<D>> SendAsync(BackChannelRequestDto<S> backChannelRequestDto);
    }
}
