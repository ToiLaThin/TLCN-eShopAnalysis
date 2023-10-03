using eShopAnalysis.StockInventoryAPI.Utilities.Result;

namespace eShopAnalysis.StockInventoryAPI.Services
{
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventory.Repository;
    using Microsoft.AspNetCore.Mvc;

    public interface IStockInventoryService
    {
        ServiceResponseDto<StockInventory> Add(StockInventory stockToAdd);

        ServiceResponseDto<StockInventory> AddNew(string productId, string productModelId, string businessKey);

        ServiceResponseDto<IEnumerable<StockInventory>> GetAll();


    }


    public class StockInventoryService : IStockInventoryService
    {
        private readonly IStockInventoryRepository _repo;
        public StockInventoryService(IStockInventoryRepository repo)
        {
            _repo = repo;
        }

        public ServiceResponseDto<StockInventory> Add(StockInventory stockToAdd)
        {
            var result = _repo.Add(stockToAdd);
            if (result != null) {
                return ServiceResponseDto<StockInventory>.Success(result);
            }
            return ServiceResponseDto<StockInventory>.Failure("Cannot add stock to repo");
        }

        public ServiceResponseDto<StockInventory> AddNew(string productId, string productModelId, string businessKey)
        {

            var result = _repo.Add(new StockInventory()
            {
                StockInventoryId = Ulid.NewUlid(),
                ProductId = productId,
                ProductModelId = productModelId,
                ProductBusinessKey = businessKey,
                CurrentQuantity = 50
            });
            if (result != null)
            {
                return ServiceResponseDto<StockInventory>.Success(result);
            }
            return ServiceResponseDto<StockInventory>.Failure("Cannot add stock to repo");
        }

        public ServiceResponseDto<IEnumerable<StockInventory>> GetAll()
        {
            var result = _repo.GetAll();
            return ServiceResponseDto<IEnumerable<StockInventory>>.Success(result);
        }
    }
}
