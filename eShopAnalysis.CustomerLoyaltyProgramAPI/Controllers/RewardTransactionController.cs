using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Models;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Service;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Controllers
{
    [Route("api/CustomerLoyaltyProgramAPI/RewardTransactionAPI")]
    [ApiController]
    public class RewardTransactionController: ControllerBase
    {
        private readonly IRewardTransactionService _rewardTransactionService;
        private readonly IMapper _mapper;

        public RewardTransactionController(IRewardTransactionService rewardTransactionService, IMapper mapper)
        {
            _rewardTransactionService = rewardTransactionService;
            _mapper = mapper;
        }

        [HttpGet("GetRewardTransactionsOfUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<RewardTransactionDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<RewardTransactionDto>>> GetRewardTransactionsOfUser(Guid userId)
        {
            var serviceResult = await _rewardTransactionService.GetRewardTransactionsOfUser(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }

            if (serviceResult.Data.Count() == 0) {
                return NoContent();
            }

            var resultDto = _mapper.Map<IEnumerable<RewardTransactionDto>>(serviceResult.Data);
            return Ok(resultDto);
        }

        //TODO convert to backchannel
        [HttpPost("BackChannel/AddRewardTransactionForApplyCoupon")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RewardTransactionDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<RewardTransactionDto>> AddRewardTransactionForApplyCoupon(
            [FromHeader] Guid userId, 
            [FromHeader] CouponDiscountType discountType,
            [FromHeader] double discountValue,
            [FromHeader] int pointTransition
            )
        {
            var serviceResult = await _rewardTransactionService.AddRewardTransactionForApplyCoupon(
                    userId,
                    discountType,
                    discountValue,
                    pointTransition);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<RewardTransactionDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("BackChannel/AddRewardTransactionForCompleteOrdering")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RewardTransaction), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<RewardTransactionDto>> AddRewardTransactionForCompleteOrdering(
            [FromHeader] Guid userId,
            [FromHeader] int pointTransition,
            [FromHeader] double orderPrice
            )
        {
            var serviceResult = await _rewardTransactionService.AddRewardTransactionForCompleteOrdering(
                    userId,
                    pointTransition, 
                    orderPrice);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<RewardTransactionDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpDelete("DeleteExistingRewardTransactionInstance")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RewardTransactionDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<RewardTransactionDto>> DeleteExistingRewardTransactionInstance(
            [FromHeader] Guid rewardTransactionId)
        {
            var serviceResult = await _rewardTransactionService.DeleteExistingRewardTransactionInstance(rewardTransactionId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<RewardTransactionDto>(serviceResult.Data);
            return Ok(resultDto);
        }
    }
}
