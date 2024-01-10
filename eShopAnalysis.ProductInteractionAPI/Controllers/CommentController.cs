using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Service;
using eShopAnalysis.ProductInteractionAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ProductInteractionAPI.Controllers
{
    [Route("api/ProductInteractionAPI/CommentAPI")]
    [ApiController]
    public class CommentController: ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpGet("GetCommentsOfUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsOfUser(Guid userId)
        {
            var serviceResult = await _commentService.GetCommentsOfUserAsync(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(serviceResult.Data);
            if (resultDto.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpGet("GetCommentsAboutProduct")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsAboutProduct(Guid productBusinessKey)
        {
            var serviceResult = await _commentService.GetCommentsAboutProductAsync(productBusinessKey);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentDto>>(serviceResult.Data);
            if (resultDto.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpPost("CommentProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CommentDto>> CommentProductFromUser(Guid userId, Guid productBusinessKey, string commentDetail)
        {
            var serviceResult = await _commentService.Add(userId, productBusinessKey, commentDetail);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Comment, CommentDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpDelete("UnCommentProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CommentDto>> UnCommentProductFromUser(Guid userId, Guid productBusinessKey)
        {
            var serviceResult = await _commentService.Remove(userId, productBusinessKey);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Comment, CommentDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpPut("UpdateCommentProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<CommentDto>> UpdateCommentProductFromUser(Guid userId, Guid productBusinessKey, string updatedCommentDetail)
        {
            var serviceResult = await _commentService.Update(userId, productBusinessKey, updatedCommentDetail);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Comment, CommentDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }
    }
}
