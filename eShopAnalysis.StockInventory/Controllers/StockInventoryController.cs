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
            var result = _service.GetAll();
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
            var result = _service.Add(stockInventory);
            if (result.IsFailed || result.IsException) {
                return NotFound(result.Error);
            }
            return Ok(result.Data);
        }

        //for any microservice want to add new stock inventory
        [HttpPost("BackChannel/AddStock")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public BackChannelResponseDto<StockInventoryDto> AddNew([FromBody] StockInventoryDto stockInventoryDtoToAdd)
        {
            var result = _service.AddNew(stockInventoryDtoToAdd.ProductId, stockInventoryDtoToAdd.ProductModelId, stockInventoryDtoToAdd.ProductBusinessKey);
            if (result.IsFailed || result.IsException) {
                return BackChannelResponseDto<StockInventoryDto>.Failure(result.Error);
            }
            var stockInventoryDto = _mapper.Map<StockInventoryDto>(result.Data);
            return BackChannelResponseDto<StockInventoryDto>.Success(stockInventoryDto);
        }

        [HttpPost("BackChannel/GetStockOfModels")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetStockOfModels([FromBody] OrderItemsStockRequestDto orderItemsStockReq)
        {
            var result = _service.GetStockOfModels(orderItemsStockReq.ProductModelIds);
            if (result.IsFailed || result.IsException) {
                return null;
            }
            return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result.Data);
        }

        [HttpPost("BackChannel/DecreaseStockItems")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> DecreaseStockItems([FromBody] IEnumerable<StockDecreaseRequestDto> stockDecreaseReqs)
        {
            if (stockDecreaseReqs == null) { throw new ArgumentNullException(nameof(stockDecreaseReqs)); }
            var result = _service.DecreaseStockItems(stockDecreaseReqs);
            if (result.IsFailed || result.IsException) {
                return null;
            }
            return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result.Data);
        }
    }
}
