using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Service
{
    public interface IStockRequestTransactionService
    {
        Task<ServiceResponseDto<IEnumerable<StockRequestTransaction>>> GetAll();

        Task<ServiceResponseDto<StockRequestTransaction>> Add(StockRequestTransaction stockTransReqToAdd);

        Task<ServiceResponseDto<string>> Truncate();
    }
}
