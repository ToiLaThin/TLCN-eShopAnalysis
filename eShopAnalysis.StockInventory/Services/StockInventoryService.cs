using eShopAnalysis.StockInventoryAPI.Utilities.Result;

namespace eShopAnalysis.StockInventoryAPI.Services
{
    using eShopAnalysis.ApiGateway.Services.BackchannelDto;
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventory.Repository;
    using eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto;

    public interface IStockInventoryService
    {
        ServiceResponseDto<StockInventory> Add(StockInventory stockToAdd);

        ServiceResponseDto<StockInventory> AddNew(string productId, string productModelId, string businessKey);

        ServiceResponseDto<IEnumerable<StockInventory>> GetAll();

        ServiceResponseDto<IEnumerable<ItemStockResponseDto>> GetStockOfModels(IEnumerable<Guid> modelIds);

        ServiceResponseDto<IEnumerable<ItemStockResponseDto>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> itemStocks);
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

        public ServiceResponseDto<IEnumerable<ItemStockResponseDto>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> decreaseReqs)
        {
            var requestedModelIds = decreaseReqs.Select(req => req.ProductModelId).ToList();
            var stocksToDecrease = _repo.GetAll()
                                        .Where(st => requestedModelIds
                                        .Contains(Guid.Parse(st.ProductModelId)));

            if (stocksToDecrease != null) {
                foreach (var stock in stocksToDecrease)
                {
                    //TODO unit of work for stock to make sure all are updated or none
                    //if multiple decrease req for one single stockItem(with same ProductModelId)
                    //will have error => we must group in the aggregator controller, also , this help reduce the payload
                    var req = decreaseReqs.Single(req => req.ProductModelId == Guid.Parse(stock.ProductModelId));
                    if (req == null) { 
                        throw new Exception("Critical error"); 
                    }
                    stock.CurrentQuantity -= req.QuantityToDecrease;
                    _repo.Update(stock);
                }
                //get result after update
                IEnumerable<ItemStockResponseDto> result = _repo.GetAll()
                                        .Where(st => requestedModelIds
                                        .Contains(Guid.Parse(st.ProductModelId)))
                                        .Select(st => {
                                            return new ItemStockResponseDto
                                            {
                                                ProductModelId = Guid.Parse(st.ProductModelId),
                                                CurrentQuantity = st.CurrentQuantity,
                                            };
                                        });

                return ServiceResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result);
            }
            return ServiceResponseDto<IEnumerable<ItemStockResponseDto>>.Failure("Error");
        }
    }
}
