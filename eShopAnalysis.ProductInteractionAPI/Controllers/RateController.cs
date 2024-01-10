using AutoMapper;
using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Service;
using eShopAnalysis.ProductInteractionAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.ProductInteractionAPI.Controllers
{

    [Route("api/ProductInteractionAPI/RateAPI")]
    [ApiController]
    public class RateController: ControllerBase
    {
        private readonly IRateService _rateService;
        private readonly IMapper _mapper;

        public RateController(IRateService rateService, IMapper mapper)
        {
            _rateService = rateService;
            _mapper = mapper;
        }

        [HttpGet("GetRatedMappingsOfUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<RateDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<RateDto>>> GetRatedMappingsOfUser(Guid userId)
        {
            var serviceResult = await _rateService.GetRatedMappingsOfUserAsync(userId);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<Rate>, IEnumerable<RateDto>>(serviceResult.Data);
            if (resultDto.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpGet("GetRatedMappingsAboutProduct")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<RateDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<RateDto>>> GetRatedMappingsAboutProduct(Guid productBusinessKey)
        {
            var serviceResult = await _rateService.GetRatedMappingsAboutProductAsync(productBusinessKey);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<Rate>, IEnumerable<RateDto>>(serviceResult.Data);
            if (resultDto.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpPost("RateProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<RateDto>> RateProductFromUser(Guid userId, Guid productBusinessKey, double rating)
        {
            var serviceResult = await _rateService.Add(userId, productBusinessKey, rating);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Rate, RateDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }

        [HttpDelete("UnRateProductFromUser")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RateDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<RateDto>> UnRateProductFromUser(Guid userId, Guid productBusinessKey)
        {
            var serviceResult = await _rateService.Remove(userId, productBusinessKey);
            ActionResult actionResultDto = (serviceResult.IsSuccess == true) ?
                                         Ok(_mapper.Map<Rate, RateDto>(serviceResult.Data)) :
                                         NotFound(serviceResult.Error);
            return actionResultDto;
        }
    }
}
