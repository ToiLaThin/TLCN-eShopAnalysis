using eShopAnalysis.Aggregator.Result;
using eShopAnalysis.Aggregator.Services.BackchannelDto;

namespace eShopAnalysis.Aggregator.Services.BackchannelServices
{
    public interface IBackChannelStockProviderRequestService
    {
        Task<BackChannelResponseDto<StockRequestTransactionDto>> AddNewStockRequestTransaction(StockRequestTransactionDto stockReqTransDtoToAdd);
    }
}
