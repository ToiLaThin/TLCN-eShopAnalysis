using eShopAnalysis.CouponSaleItemAPI.Data;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.CouponSaleItemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet("GetAllCoupons")]
        public IEnumerable<Coupon> GetAllCoupons() { 
            var result = _couponService.GetAll();
            return result;
        }

        [HttpPost("AddCoupon")]
        public async Task<Coupon> AddCoupon([FromBody] Coupon coupon)
        {
            var result = await _couponService.Add(coupon);
            return result;
        }

        
    }
}
