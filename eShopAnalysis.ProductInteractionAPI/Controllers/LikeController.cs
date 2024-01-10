using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Service;
using eShopAnalysis.ProductInteractionAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ProductInteractionAPI.Controllers
{
    [Route("api/ProductInteractionAPI/LikeAPI")]
    [ApiController]
    public class LikeController: ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly IMapper _mapper;

        public LikeController(ILikeService likeService, IMapper mapper)
        {
            _likeService = likeService;
            _mapper = mapper;
        }

        [HttpGet("GetUserProductLikedMappingsOfUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<LikeDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserProductLikedMappingsOfUser(Guid userId)
        {
            var serviceResult = await _likeService.GetLikedMappingsOfUserAsync(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<Like>, IEnumerable<LikeDto>>(serviceResult.Data);
            if (resultDto.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpPost("LikeProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LikeDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<LikeDto>> LikeProductFromUser(Guid userId, Guid productBusinessKey)
        {
            var serviceResult = await _likeService.Add(userId, productBusinessKey);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Like, LikeDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpDelete("UnLikeProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(LikeDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<LikeDto>> UnLikeProductFromUser(Guid userId, Guid productBusinessKey)
        {
            var serviceResult = await _likeService.Remove(userId, productBusinessKey);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Like, LikeDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }
    }
}
