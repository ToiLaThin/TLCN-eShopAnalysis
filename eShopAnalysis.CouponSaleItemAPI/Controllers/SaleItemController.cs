using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CouponSaleItemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleItemController : ControllerBase
    {
        private readonly ISaleItemService _saleItemService;
        public SaleItemController(ISaleItemService saleItemService)
        {
            _saleItemService = saleItemService;
        }

        [HttpGet("GetAllSaleItems")]
        public IEnumerable<SaleItem> GetAllSaleItems()
        {
            var result = _saleItemService.GetAll();
            return result;
        }

        [HttpPost("AddSaleItem")]
        public async Task<SaleItem> AddSaleItem([FromBody] SaleItem saleItem)
        {
            var result = await _saleItemService.Add(saleItem);
            return result;
        }
    }
}
