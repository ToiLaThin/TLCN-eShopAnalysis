using eShopAnalysis.StockInventoryAPI.Utilities.Result;

namespace eShopAnalysis.StockInventoryAPI.Services
{
    using eShopAnalysis.ApiGateway.Services.BackchannelDto;
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventory.Repository;
    using Microsoft.AspNetCore.Mvc;

    public interface IStockInventoryService
    {
        ServiceResponseDto<StockInventory> Add(StockInventory stockToAdd);

        ServiceResponseDto<StockInventory> AddNew(string productId, string productModelId, string businessKey);

        ServiceResponseDto<IEnumerable<StockInventory>> GetAll();

        ServiceResponseDto<IEnumerable<ItemStockResponseDto>> GetStockOfModels(IEnumerable<Guid> modelIds);
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

        public ServiceResponseDto<IEnumerable<ItemStockResponseDto>> GetStockOfModels(IEnumerable<Guid> modelIds)
        {            
            var stockOfModelIds = _repo.GetAll().Where(st => modelIds.Any(m => m == Guid.Parse(st.ProductModelId)));
            List<ItemStockResponseDto> result = new List<ItemStockResponseDto>();
            foreach (var stockOfModel in stockOfModelIds) {
                ItemStockResponseDto returnItemStock = new() {
                    ProductModelId = Guid.Parse(stockOfModel.ProductModelId),
                    CurrentQuantity = stockOfModel.CurrentQuantity
                };
                result.Add(returnItemStock);
            }
            return ServiceResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result);
        }
    }
}
