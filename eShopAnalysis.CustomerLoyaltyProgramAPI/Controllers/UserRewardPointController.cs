using AutoMapper;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Dto;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Service;
using eShopAnalysis.CustomerLoyaltyProgramAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.CustomerLoyaltyProgramAPI.Controllers
{
    [Route("api/CustomerLoyaltyProgramAPI/UserRewardPointAPI")]
    [ApiController]
    public class UserRewardPointController: ControllerBase
    {
        private readonly IUserRewardPointService _userRewardPointService;
        private readonly IMapper _mapper;

        public UserRewardPointController(IUserRewardPointService userRewardPointService, IMapper mapper)
        {
            _userRewardPointService = userRewardPointService;
            _mapper = mapper;
        }

        [HttpGet("GetRewardPointOfUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserRewardPointDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<UserRewardPointDto>> GetRewardPointOfUser(Guid userId)
        {
            var serviceResult = await _userRewardPointService.GetRewardPointOfUser(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<UserRewardPointDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("AddUserRewardInstance")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserRewardPointDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<UserRewardPointDto>> AddUserRewardInstance([FromHeader] Guid userId, [FromHeader] int initRewardPoint) {
            var serviceResult = await _userRewardPointService.AddInstance(userId, initRewardPoint);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<UserRewardPointDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpDelete("DeleteUserRewardInstance")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(UserRewardPointDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<UserRewardPointDto>> DeleteUserRewardInstance([FromHeader] Guid userId)
        {
            var serviceResult = await _userRewardPointService.DeleteExistingInstance(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<UserRewardPointDto>(serviceResult.Data);
            return Ok(resultDto);
        }
    }
}
