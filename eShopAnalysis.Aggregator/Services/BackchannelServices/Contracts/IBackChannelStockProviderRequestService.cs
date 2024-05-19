using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;
using eShopAnalysis.Aggregator.Services.BackchannelDto.StockProviderRequest;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelStockProviderRequestService
    {
        Task<BackChannelResponseDto<StockRequestTransactionDto>> AddNewStockRequestTransaction(StockRequestTransactionDto stockReqTransDtoToAdd);

        Task<BackChannelResponseDto<IEnumerable<StockItemRequestMetaResponseDto>>> GetStockItemRequestMetasWithProductModelIds(IEnumerable<Guid> productModelIds);
    }
}
