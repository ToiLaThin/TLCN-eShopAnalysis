using AutoMapper;
using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;
using eShopAnalysis.StockProviderRequestAPI.Service;
using eShopAnalysis.StockProviderRequestAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace eShopAnalysis.StockProviderRequestAPI.Controllers
{
    [Route("api/StockProviderRequestAPI/ProviderRequirementAPI")]
    [ApiController]
    public class ProviderRequirementController : ControllerBase
    {
        private readonly IProviderRequirementService _service;
        private readonly IMapper _mapper;

        public ProviderRequirementController(IProviderRequirementService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("GetAllProviderRequirements")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<ProviderRequirementDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<ProviderRequirementDto>>> GetAllProviderRequirements()
        {
            var serviceResult = await _service.GetAll();
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<ProviderRequirement>, IEnumerable<ProviderRequirementDto>>(serviceResult.Data);
            if (resultDto?.Count() <= 0) {
                return NoContent();
            }
            return Ok(resultDto);
        }

        [HttpPost("AddNewProviderRequirement")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProviderRequirementDto), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<ProviderRequirementDto>> AddNewProviderRequirement([FromBody] ProviderRequirement providerReq)
        {
            var serviceResult = await _service.Add(providerReq);
            if (serviceResult.IsFailed)
            {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<ProviderRequirement, ProviderRequirementDto>(serviceResult.Data);
            return Ok(resultDto);
        }

        [HttpPost("BulkAddNewProviderRequirements")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<string>> BulkAddNewProviderRequirements([FromBody] IEnumerable<ProviderRequirement> providerReqs)
        {
            var serviceResult = await _service.AddRange(providerReqs);
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            return Ok(serviceResult.Data);
        }

        [HttpDelete("TruncateProviderRequirement")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<string>> TruncateProviderRequirement()
        {
            var serviceResult = await _service.Truncate();
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            return Ok(serviceResult.Data);
        }

        [HttpPost("BackChannel/GetStockItemRequestMetasWithProductModelIds")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<StockItemRequestMetaDto>>> GetStockItemRequestMetasWithProductModelIds([FromBody] IEnumerable<Guid> productModelIds)
        {
            if (productModelIds == null || productModelIds.Count() <= 0 ) { 
                throw new ArgumentException(nameof(productModelIds));
            }
            var serviceResult = await _service.GetStockItemRequestMetasWithProductModelIds(productModelIds);
            if (serviceResult.IsFailed) {
                return BackChannelResponseDto<IEnumerable<StockItemRequestMetaDto>>.Failure(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<StockItemRequestMeta>, IEnumerable<StockItemRequestMetaDto>>(serviceResult.Data);
            return BackChannelResponseDto<IEnumerable<StockItemRequestMetaDto>>.Success(resultDto);
        }
    }
}