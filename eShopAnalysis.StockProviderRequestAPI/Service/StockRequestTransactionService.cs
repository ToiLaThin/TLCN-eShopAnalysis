using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;
using eShopAnalysis.StockProviderRequestAPI.Repository;

namespace eShopAnalysis.StockProviderRequestAPI.Service
{
    public class StockRequestTransactionService : IStockRequestTransactionService
    {
        private readonly IStockRequestTransactionRepository _stockTransReqRepo;

        public StockRequestTransactionService(IStockRequestTransactionRepository stockTransReqRepo)
        {
            _stockTransReqRepo = stockTransReqRepo;
        }
        public async Task<ServiceResponseDto<StockRequestTransaction>> Add(StockRequestTransaction stockTransReqToAdd)
        {
            if (stockTransReqToAdd.StockRequestTransactionId != null) {
                StockRequestTransaction stockTransReqToFind = await _stockTransReqRepo.GetAsync(stockTransReqToAdd.StockRequestTransactionId);
                if (stockTransReqToFind != null) {
                    return ServiceResponseDto<StockRequestTransaction>.Failure("cannot add stockTransReqToAdd  because stockTransReqToAdd with same stockReqTrans id already exist");
                }
            }

            //stockRequestTrans is only identified by id, so even if all other thing is same, it's still valid, we do not need to find
            var stockTransReqAdded = await _stockTransReqRepo.AddAsync(stockTransReqToAdd);
            if (stockTransReqAdded == null) {
                return ServiceResponseDto<StockRequestTransaction>.Failure("Cannot added stockTransReqToAdd because cannot find it, please check repo result");
            }

            return ServiceResponseDto<StockRequestTransaction>.Success(stockTransReqAdded);
        }

        public async Task<ServiceResponseDto<IEnumerable<StockRequestTransaction>>> GetAll()
        {
            var allStockTransReqs = _stockTransReqRepo.GetAllAsQueryable().ToList();
            if (allStockTransReqs == null)
            {
                return ServiceResponseDto<IEnumerable<StockRequestTransaction>>.Failure("result is null");
            }
            return ServiceResponseDto<IEnumerable<StockRequestTransaction>>.Success(allStockTransReqs);
        }

        public async Task<ServiceResponseDto<string>> Truncate()
        {
            var isSuccess = await _stockTransReqRepo.DeleteAllAsync();
            if (!isSuccess)
            {
                return ServiceResponseDto<string>.Failure("failed to truncate all document");
            }
            return ServiceResponseDto<string>.Success("truncated all document");
        }
    }
}
