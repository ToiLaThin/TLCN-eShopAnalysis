using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Service;
using eShopAnalysis.ProductInteractionAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ProductInteractionAPI.Controllers
{
    [Route("api/ProductInteractionAPI/BookmarkAPI")]
    [ApiController]
    public class BookmarkController: ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly IMapper _mapper;

        public BookmarkController(IBookmarkService bookmarkService, IMapper mapper)
        {
            _bookmarkService = bookmarkService;
            _mapper = mapper;
        }

        [HttpGet("GetProductBookmarkedOfUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<BookmarkDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<BookmarkDto>>> GetProductBookmarkedOfUser([FromHeader] Guid userId)
        {
            var serviceResult = await _bookmarkService.GetBookmarksOfUserAsync(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<Bookmark>, IEnumerable<BookmarkDto>>(serviceResult.Data);
            if (resultDto.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpPost("BookmarkProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BookmarkDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<BookmarkDto>> BookmarkProductFromUser([FromHeader] Guid userId, [FromHeader] Guid productBusinessKey)
        {
            var serviceResult = await _bookmarkService.Add(userId, productBusinessKey);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Bookmark, BookmarkDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpDelete("UnBookmarkProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BookmarkDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<BookmarkDto>> UnBookmarkProductFromUser([FromHeader] Guid userId, [FromHeader] Guid productBusinessKey)
        {
            var serviceResult = await _bookmarkService.Remove(userId, productBusinessKey);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Bookmark, BookmarkDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }
    }
}
