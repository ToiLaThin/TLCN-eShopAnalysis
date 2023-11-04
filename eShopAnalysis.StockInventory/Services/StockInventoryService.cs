using eShopAnalysis.StockInventoryAPI.Utilities.Result;

namespace eShopAnalysis.StockInventoryAPI.Services
{
    using eShopAnalysis.ApiGateway.Services.BackchannelDto;
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventory.Repository;
    using eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto;

    public interface IStockInventoryService
    {
        Task<ServiceResponseDto<StockInventory>> Add(StockInventory stockToAdd);

        Task<ServiceResponseDto<StockInventory>> AddNew(string productId, string productModelId, string businessKey);

        Task<ServiceResponseDto<IEnumerable<StockInventory>>> GetAll();

        Task<ServiceResponseDto<IEnumerable<ItemStockResponseDto>>> GetStockOfModels(IEnumerable<Guid> modelIds);

        Task<ServiceResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> itemStocks);
    }


    public class StockInventoryService : IStockInventoryService
    {
        private readonly IStockInventoryRepository _repo;
        public StockInventoryService(IStockInventoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ServiceResponseDto<StockInventory>> Add(StockInventory stockToAdd)
        {
            var result = await _repo.AddAsync(stockToAdd);
            if (result != null) {
                return ServiceResponseDto<StockInventory>.Success(result);
            }
            return ServiceResponseDto<StockInventory>.Failure("Cannot add stock to repo");
        }

        public async Task<ServiceResponseDto<StockInventory>> AddNew(string productId, string productModelId, string businessKey)
        {

            var result = await _repo.AddAsync(new StockInventory()
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

        public async Task<ServiceResponseDto<IEnumerable<StockInventory>>> GetAll()
        {
            //TODO do not get all but by some cond, this is just  for testing
            var result = _repo.GetAsQueryable().ToList();
            return ServiceResponseDto<IEnumerable<StockInventory>>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<ItemStockResponseDto>>> GetStockOfModels(IEnumerable<Guid> modelIds)
        {            
            var stockOfModelIds = _repo.GetAsQueryable()
                                       .Where(st => modelIds.Any(m => m == Guid.Parse(st.ProductModelId)))
                                       .ToList();
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

        public async Task<ServiceResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems(IEnumerable<StockDecreaseRequestDto> decreaseReqs)
        {
            var requestedModelIds = decreaseReqs.Select(req => req.ProductModelId).ToList();
            var stocksToDecrease = _repo.GetAsQueryable()
                                        .Where(st => requestedModelIds
                                        .Contains(Guid.Parse(st.ProductModelId)))
                                        .ToList();

            if (stocksToDecrease != null) {
                foreach (var stock in stocksToDecrease)
                {
                    //TODO unit of work for stock to make sure all are updated or none
                    //if multiple decrease req for one single stockItem(with same ProductModelId)
                    //will have error => we must group in the aggregator controller, also , this help reduce the payload(done)
                    var req = decreaseReqs.Single(req => req.ProductModelId == Guid.Parse(stock.ProductModelId));
                    if (req == null) { 
                        throw new Exception("Critical error"); 
                    }
                    stock.CurrentQuantity -= req.QuantityToDecrease;
                    _repo.UpdateAsync(stock); //async here help improve performance, not use await to further improve performance
                }
                //get result after update
                IEnumerable<ItemStockResponseDto> result = _repo.GetAsQueryable()
                                        .Where(st => requestedModelIds
                                        .Contains(Guid.Parse(st.ProductModelId)))
                                        .ToList()
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
