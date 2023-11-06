using eShopAnalysis.StockInventory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.StockInventory.Controllers
{
    using AutoMapper;
    using eShopAnalysis.ApiGateway.Services.BackchannelDto;
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventoryAPI.Dto;
    using eShopAnalysis.StockInventoryAPI.Dto.BackchannelDto;
    using eShopAnalysis.StockInventoryAPI.Services;
    using eShopAnalysis.StockInventoryAPI.Utilities.Behaviors;
    using eShopAnalysis.StockInventoryAPI.Utilities.Result;
    using System.Collections.Generic;

    [Route("api/StockInventoryAPI/StockSnapshotAPI")]
    [ApiController]
    public class StockInventoryController : ControllerBase
    {
        private readonly IStockInventoryService _service;
        private readonly IMapper _mapper;
        public StockInventoryController(IStockInventoryService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<StockInventory>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<StockInventory>>> GetAll()
        {
            var result = await _service.GetAll();
            if (result.Data.Count() <= 0) {
                return NotFound("no stock in warehouse");
            }
            return Ok(result.Data);
        }

        [HttpPost("AddStock")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<StockInventory>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<StockInventory>> Add(StockInventory stockInventory) {
            var result = await _service.Add(stockInventory);
            if (result.IsFailed || result.IsException) {
                return NotFound(result.Error);
            }
            return Ok(result.Data);
        }

        //for any microservice want to add new stock inventory
        [HttpPost("BackChannel/AddNewStockInventories")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<StockInventoryDto>>> AddNewStockInventories([FromBody] IEnumerable<StockInventoryDto> stockInventoryDtosToAdd)
        {
            if (stockInventoryDtosToAdd == null || stockInventoryDtosToAdd.Count() <= 0) {
                throw new ArgumentNullException(nameof(stockInventoryDtosToAdd));
            }
            var stockInventoriesToAdd = _mapper.Map<IEnumerable<StockInventory>>(stockInventoryDtosToAdd);
            var result = await _service.AddNewStocks(stockInventoriesToAdd);
            if (result.IsFailed || result.IsException) {
                return BackChannelResponseDto<IEnumerable<StockInventoryDto>>.Failure(result.Error);
            }
            var stockInventoryDtos = _mapper.Map<IEnumerable<StockInventoryDto>>(result.Data);
            return BackChannelResponseDto<IEnumerable<StockInventoryDto>>.Success(stockInventoryDtos);
        }

        [HttpPost("BackChannel/GetStockOfModels")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetStockOfModels([FromBody] OrderItemsStockRequestDto orderItemsStockReq)
        {
            if (orderItemsStockReq == null) {
                return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Exception("argument is null");
            }
            if (orderItemsStockReq.ProductModelIds.Count() <= 0) {
                return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Failure("argument is not valid");
            }
            var result = await _service.GetStockOfModels(orderItemsStockReq.ProductModelIds);
            if (result.IsFailed || result.IsException) {
                return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Failure(result.Error);
            }
            return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result.Data);
        }

        [HttpPost("BackChannel/DecreaseStockItems")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems([FromBody] IEnumerable<StockDecreaseRequestDto> stockDecreaseReqs)
        {
            if (stockDecreaseReqs == null) { throw new ArgumentNullException(nameof(stockDecreaseReqs)); }
            var result = await _service.DecreaseStockItems(stockDecreaseReqs);
            if (result.IsFailed || result.IsException) {
                return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Failure(result.Error);
            }
            return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result.Data);
        }
    }
}
