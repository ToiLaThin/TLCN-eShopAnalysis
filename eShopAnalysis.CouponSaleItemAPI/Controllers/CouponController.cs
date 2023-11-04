using AutoMapper;
using eShopAnalysis.CouponSaleItemAPI.Application.BackchannelDto;
using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service;
using eShopAnalysis.CouponSaleItemAPI.Utilities.Behaviors;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCoupons() { 
            var serviceResult = await _couponService.GetAll();
            if (serviceResult.Data.Count() <= 0) {
                return NotFound("none coupon exist");
            }
            var resultDto = _mapper.Map<IEnumerable<CouponDto>>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpGet("GetAllCouponsUsedByUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllCouponsUsedByUser(Guid userId)
        {
            var serviceResult = await _couponService.GetCouponUsedByUser(userId);
            if (serviceResult.Data.Count() <= 0) {
                return NoContent();
            }
            var resultDto = _mapper.Map<IEnumerable<CouponDto>>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpGet("GetAllActiveCouponsNotUsedByUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<CouponDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<CouponDto>>> GetAllActiveCouponsNotUsedByUser([FromQuery] Guid userId)
        {
            var serviceResult = await _couponService.GetActiveCouponsNotUsedByUser(userId);
            if (serviceResult.Data.Count() <= 0) {
                return NoContent();
            }
            var resultDto = _mapper.Map<IEnumerable<CouponDto>>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("AddCoupon")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CouponDto>> AddCoupon([FromBody] Coupon coupon)
        {
            var serviceResult = await _couponService.Add(coupon);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<CouponDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("AddCouponUsedByUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        //thay vi return CouponUser hay CouponUserDto, ta return Coupon dc foreign toi
        public async Task<ActionResult<CouponDto>> AddCouponUsedByUser([FromQuery] Guid couponId, [FromQuery] Guid userId)
        {
            var serviceResult = await _couponService.MarkUserUsedCoupon(userId: userId, couponId: couponId);
            if (serviceResult.IsFailed)
            {
                return NotFound(serviceResult.Error);
            }
            //sẽ trả về null neu repo.Get ko co Include
            var resultDto = _mapper.Map<CouponDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpGet("BackChannel/RetrieveCouponWithCode")]
        //for swagger , this must be post not get request(cannot have body), but without swagger i think it's ok
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<CouponDto>> RetrieveCouponWithCode([FromBody] RetrieveCouponWithCodeRequestDto retrieveCouponWithCodeRequestDto)
        {
            var serviceResult = await _couponService.RetrieveValidCouponWithCode(retrieveCouponWithCodeRequestDto.CouponCode);
            if (serviceResult.IsFailed) 
            {
                return BackChannelResponseDto<CouponDto>.Failure(serviceResult.Error);
            }
            else if (serviceResult.IsException)
            {
                return BackChannelResponseDto<CouponDto>.Exception(serviceResult.Error);
            }
            var resultDto = _mapper.Map<CouponDto>(serviceResult.Data);
            return BackChannelResponseDto<CouponDto>.Success(resultDto);
        }

    }
}
