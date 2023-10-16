using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Application.BackchannelDto;
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

        [HttpGet("GetAllCouponsUsedByUser")]
        public IEnumerable<CouponDto> GetAllCouponsUsedByUser(Guid userId)
        {
            var result = _couponService.GetCouponUsedByUser(userId);
            if (result.IsFailed)
            {
                return null;
            }
            var resultDto = _mapper.Map<IEnumerable<CouponDto>>(result.Data);
            return resultDto;
        }

        [HttpGet("GetAllActiveCouponsNotUsedByUser")]
        public IEnumerable<CouponDto> GetAllActiveCouponsNotUsedByUser([FromQuery] Guid userId)
        {
            var result = _couponService.GetActiveCouponsNotUsedByUser(userId);
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

        [HttpPost("AddCouponUsedByUser")]
        //thay vi return CouponUser hay CouponUserDto, ta return Coupon dc foreign toi
        public async Task<CouponDto> AddCouponUsedByUser([FromQuery] Guid couponId, [FromQuery] Guid userId)
        {
            var result = await _couponService.MarkUserUsedCoupon(userId: userId, couponId: couponId);
            if (result.IsFailed)
            {
                return null;
            }
            //sẽ trả về null neu repo.Get ko co Include
            var resultDto = _mapper.Map<CouponDto>(result.Data);
            return resultDto;
        }

        [HttpGet("BackChannel/RetrieveCouponWithCode")]
        public BackChannelResponseDto<CouponDto> RetrieveCouponWithCode([FromBody] RetrieveCouponWithCodeRequestDto retrieveCouponWithCodeRequestDto)
        {
            var result = _couponService.RetrieveValidCouponWithCode(retrieveCouponWithCodeRequestDto.CouponCode);
            if (result.IsFailed)
            {
                return BackChannelResponseDto<CouponDto>.Failure(result.Error);
            }
            else if (result.IsException)
            {
                return BackChannelResponseDto<CouponDto>.Exception(result.Error);
            }
            var resultDto = _mapper.Map<CouponDto>(result.Data);
            return BackChannelResponseDto<CouponDto>.Success(resultDto);
        }

    }
}
