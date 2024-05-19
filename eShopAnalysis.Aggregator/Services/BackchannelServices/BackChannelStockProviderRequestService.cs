using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelDto.StockProviderRequest;
using eShopAnalysis.Aggregator.Utilities;
using Microsoft.Extensions.Options;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public class BackChannelStockProviderRequestService : IBackChannelStockProviderRequestService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<BackChannelCommunication> _backChannelUrls;
        public BackChannelStockProviderRequestService(
            IOptions<BackChannelCommunication> backChannelUrls,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _backChannelUrls = backChannelUrls;
        }
        public async Task<BackChannelResponseDto<StockRequestTransactionDto>> AddNewStockRequestTransaction(StockRequestTransactionDto stockReqTransDtoToAdd)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<StockRequestTransactionDto, StockRequestTransactionDto>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<StockRequestTransactionDto>()
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.StockRequestTransactionAPIBaseUri}/AddNewStockRequestTransaction",
                Data = stockReqTransDtoToAdd
            });
            return result;
        }

        public async Task<BackChannelResponseDto<IEnumerable<StockItemRequestMetaResponseDto>>> GetStockItemRequestMetasWithProductModelIds(IEnumerable<Guid> productModelIds)
        {
            var baseService = _serviceProvider.GetRequiredService<IBackChannelBaseService<IEnumerable<Guid>,  IEnumerable<StockItemRequestMetaResponseDto>>>();
            var result = await baseService.SendAsync(new BackChannelRequestDto<IEnumerable<Guid>>
            {
                ApiType = ApiType.POST,
                Url = $"{_backChannelUrls.Value.ProviderRequirementAPIBaseUri}/GetStockItemRequestMetasWithProductModelIds",
                Data = productModelIds
            });
            return result;
        }
    }
}
