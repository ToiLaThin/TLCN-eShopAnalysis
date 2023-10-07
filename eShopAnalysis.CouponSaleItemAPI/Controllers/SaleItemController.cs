using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
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
        public IEnumerable<SaleItemDto> GetAllSaleItems()
        {
            var result = _saleItemService.GetAll();
            if (result.IsFailed)
            {
                return null;
            }
            var resultDto = _mapper.Map<IEnumerable<SaleItemDto>>(result.Data);
            return resultDto;
        }

        [HttpPost("AddSaleItem")]
        public async Task<SaleItemDto> AddSaleItem([FromBody] SaleItem saleItem)
        {
            var result = await _saleItemService.Add(saleItem);
            if (result.IsFailed)
            {
                return null;
            }
            var resultDto = _mapper.Map<SaleItemDto>(result.Data);
            return resultDto;
        }
    }
}
