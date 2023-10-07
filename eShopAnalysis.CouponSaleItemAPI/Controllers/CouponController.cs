using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.CouponSaleItemAPI.Controllers
{
    [Route("api/CouponSaleItemAPI/CouponAPI")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        public CouponController(ICouponService couponService, IMapper mapper)
        {
            _couponService = couponService;
            _mapper = mapper;
        }

        [HttpGet("GetAllCoupons")]
        public IEnumerable<CouponDto> GetAllCoupons() { 
            var result = _couponService.GetAll();
            if (result.IsFailed)
            {
                return null;
            }
            var resultDto = _mapper.Map<IEnumerable<CouponDto>>(result.Data);
            return resultDto;
        }

        [HttpPost("AddCoupon")]
        public async Task<CouponDto> AddCoupon([FromBody] Coupon coupon)
        {
            var result = await _couponService.Add(coupon);
            if (result.IsFailed)
            {
                return null;
            }
            var resultDto = _mapper.Map<CouponDto>(result.Data);
            return resultDto;
        }

        
    }
}
