using eShopAnalysis.StockInventory.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.StockInventory.Controllers
{
    using AutoMapper;
    using eShopAnalysis.ApiGateway.Services.BackchannelDto;
    using eShopAnalysis.StockInventory.Models;
    using eShopAnalysis.StockInventoryAPI.Dto;
    using eShopAnalysis.StockInventoryAPI.Services;
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
        public IEnumerable<StockInventory> GetAll()
        {
            var result = _service.GetAll();
            if (result.IsFailed)
            {
                return null;
            }
            return result.Data;
        }

        [HttpPost("AddStock")]
        public StockInventory Add(StockInventory stockInventory) {
            var result = _service.Add(stockInventory);
            if (result.IsFailed)
            {
                return null;
            }
            return result.Data;
        }

        //for any microservice want to add new stock inventory
        [HttpPost("BackChannel/AddStock")]
        public BackChannelResponseDto<StockInventoryDto> AddNew([FromBody] StockInventoryDto stockInventoryDtoToAdd)
        {
            var result = _service.AddNew(stockInventoryDtoToAdd.ProductId, stockInventoryDtoToAdd.ProductModelId, stockInventoryDtoToAdd.ProductBusinessKey);
            if (result.IsFailed) {
                return null;
            }
            var stockInventoryDto = _mapper.Map<StockInventoryDto>(result.Data);
            return BackChannelResponseDto<StockInventoryDto>.Success(stockInventoryDto);
        }

        [HttpPost("BackChannel/GetStockOfModels")]
        public async Task<BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>> GetStockOfModels([FromBody] OrderItemsStockRequestDto orderItemsStockReq)
        {
            var result = _service.GetStockOfModels(orderItemsStockReq.ProductModelIds);
            if (result.IsFailed) {
                return null;
            }
            return BackChannelResponseDto<IEnumerable<ItemStockResponseDto>>.Success(result.Data);
        }
    }
}
