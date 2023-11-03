using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
using eShopAnalysis.CouponSaleItemAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CouponSaleItemAPI.Controllers
{
    [Route("api/CouponSaleItemAPI/SaleItemAPI")]
    [ApiController]
    public class SaleItemController : ControllerBase
    {
        private readonly ISaleItemService _saleItemService;
        private readonly IMapper _mapper;
        public SaleItemController(ISaleItemService saleItemService, IMapper mapper)
        {
            _saleItemService = saleItemService;
            _mapper = mapper;
        }

        [HttpGet("GetAllSaleItems")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<SaleItemDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<SaleItemDto>>> GetAllSaleItems()
        {
            var serviceResult = _saleItemService.GetAll();
            if (serviceResult.Data.Count() <= 0) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<SaleItemDto>>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("AddSaleItem")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(SaleItemDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SaleItemDto>> AddSaleItem([FromBody] SaleItem saleItem)
        {
            var serviceResult = await _saleItemService.Add(saleItem);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<SaleItemDto>(serviceResult.Data);
            return Ok(resultDto);
        }
    }
}
