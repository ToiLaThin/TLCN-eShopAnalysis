using eShopAnalysis.StockInventoryAPI.Utilities.Result;

namespace eShopAnalysis.StockInventoryAPI.Services
{
    using eShopAnalysis.ApiGateway.Services.BackchannelDto;
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventory.Repository;
    using eShopAnalysis.StockInventoryAPI.Dto;
    using eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    public interface IStockInventoryService
    {
        Task<ServiceResponseDto<StockInventory>> Add(StockInventory stockToAdd);

        Task<ServiceResponseDto<IEnumerable<StockInventory>>> AddNewStocks(IEnumerable<StockInventory> stockInventories);

        Task<ServiceResponseDto<IEnumerable<StockInventory>>> GetAll();

        Task<ServiceResponseDto<IEnumerable<StockInventory>>> UpdateIdsAfterProductModelPriceChanged(Guid oldProductId, Guid newProductId, Guid oldProductModelId, Guid newProductModelId);

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

        public async Task<ServiceResponseDto<IEnumerable<StockInventory>>> AddNewStocks(IEnumerable<StockInventory> stockInventories)
        {
            var productIdsToCheck = stockInventories.Select(x => x.ProductId).Distinct(); //can have id of the same product for models, so distince to avoid duplication
            var productModelIdsToCheck = stockInventories.Select(x => x.ProductModelId).Distinct();
            var productIdsInDb = _repo.GetAsQueryable()
                                      .ToList()//to list truoc rồi mới select được
                                      .Select(st => st.ProductId)
                                      .Distinct(); 
            var productModelIdsInDb = _repo.GetAsQueryable()
                                           .ToList()
                                           .Select(st => st.ProductModelId)
                                           .Distinct();
            int numProductIdsExisted = productIdsInDb.Intersect(productIdsToCheck)
                                                     .Count();
            int numProductModelIds = productModelIdsInDb.Intersect(productModelIdsToCheck)
                                                        .Count();
            if (numProductIdsExisted > 0 || numProductModelIds > 0) {
                return ServiceResponseDto<IEnumerable<StockInventory>>.Failure("one or more product ids or product model ids already exists");
            }

            List<StockInventory> result = new() { };
            foreach (var stock in stockInventories) {
                var stockAdded = await _repo.AddAsync(new StockInventory()
                {
                    StockInventoryId = Ulid.NewUlid(),
                    ProductId = stock.ProductId,
                    ProductModelId = stock.ProductModelId,
                    ProductBusinessKey = stock.ProductBusinessKey,
                    CurrentQuantity = 50
                });
                //TODO can add unit of work
                if (stockAdded == null){
                    return ServiceResponseDto<IEnumerable<StockInventory>>.Failure("cannot add one of the stock dto");
                    //throw new Exception(""cannot add one of the stock dto"");
                }
                result.Add(stockAdded);
            }
            return ServiceResponseDto<IEnumerable<StockInventory>>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<StockInventory>>> GetAll()
        {
            //TODO do not get all but by some cond, this is just  for testing
            var result = _repo.GetAsQueryable().ToList();
            return ServiceResponseDto<IEnumerable<StockInventory>>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<ItemStockResponseDto>>> GetStockOfModels(IEnumerable<Guid> modelIds)
        {
            IEnumerable<string> modelIdsStr = modelIds.Select(x => x.ToString()); //convert to string so we do not have to use Guid.Parse in the contain or any method(have errro)
            var stockOfModelIds = _repo.GetAsQueryable()
                                       .Where(st => modelIdsStr.Contains(st.ProductModelId))
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
            var requestedModelIds = decreaseReqs.Select(req => req.ProductModelId.ToString()).ToList();
            var stocksToDecrease = _repo.GetAsQueryable()
                                        .Where(st => requestedModelIds.Contains(st.ProductModelId))
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
                                        .Contains(st.ProductModelId))
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

        public async Task<ServiceResponseDto<IEnumerable<StockInventory>>> UpdateIdsAfterProductModelPriceChanged(Guid oldProductId, Guid newProductId, Guid oldProductModelId, Guid newProductModelId)
        {
            //find all stockInventory with productId is oldProductId, then replace them all with newProductId
            string oldProductIdStr = oldProductId.ToString();
            string newProductIdStr = newProductId.ToString();
            //string oldProductModelIdStr = oldProductModelId.ToString();
            //string newProductModelIdStr = newProductModelId.ToString();

            var stockToUpdates = _repo.GetAsQueryable().Where(st => st.ProductId == oldProductIdStr).ToList();
            if (stockToUpdates.Count <= 0) {
                return ServiceResponseDto<IEnumerable<StockInventory>>.Failure("no stock inventory");
            }

            foreach(var st in  stockToUpdates) {
                st.ProductId = newProductId.ToString();
                //in those stockInventory, the stock with productModelId is oldProductModelId, replace that with newProductModelId
                if (st.ProductModelId == oldProductModelId.ToString()) {
                    st.ProductModelId = newProductModelId.ToString();
                }

                //save changes asynchronously
                await _repo.UpdateAsync(st);
            }

            var stockAfterUpdated = _repo.GetAsQueryable().Where(st => st.ProductId ==  newProductIdStr).ToList();
            return ServiceResponseDto<IEnumerable<StockInventory>>.Success(stockAfterUpdated);
        }
    }
}
