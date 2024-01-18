using AutoMapper;
using eShopAnalysis.StockProviderRequestAPI.Dto;
using eShopAnalysis.StockProviderRequestAPI.Models;
using eShopAnalysis.StockProviderRequestAPI.Service;
using eShopAnalysis.StockProviderRequestAPI.Utilities.Behaviors;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.StockProviderRequestAPI.Controllers
{
    [Route("api/StockProviderRequestAPI/StockRequestTransactionAPI")]
    [ApiController]
    public class StockRequestTransactionController: ControllerBase
    {
        private readonly IStockRequestTransactionService _service;
        private readonly IMapper _mapper;

        public StockRequestTransactionController(IStockRequestTransactionService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("GetAllStockRequestTransactions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<StockRequestTransactionDto>), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<StockRequestTransactionDto>>> GetAllStockRequestTransactions()
        {
            var serviceResult = await _service.GetAll();
            if (serviceResult.IsFailed) {
                return NotFound(serviceResult.Error);
            }
            var resultDto = _mapper.Map<IEnumerable<StockRequestTransaction>, IEnumerable<StockRequestTransactionDto>>(serviceResult.Data);
            if (resultDto?.Count() <= 0)
            {
                return NoContent();
            }
            return Ok(resultDto);
        }        

        [HttpDelete("TruncateStockRequestTransaction")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<string>> TruncateStockRequestTransaction()
        {
            var serviceResult = await _service.Truncate();
            if (serviceResult.IsFailed)
            {
                return NotFound(serviceResult.Error);
            }
            return Ok(serviceResult.Data);
        }

        [HttpPost("BackChannel/AddNewStockRequestTransaction")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<StockRequestTransactionDto>> AddNewStockRequestTransaction([FromBody] StockRequestTransactionDto stockRequestTransactionDtoToAdd)
        {
            var stockRequestTransactionToAdd = _mapper.Map<StockRequestTransactionDto, StockRequestTransaction>(stockRequestTransactionDtoToAdd);
            var serviceResult = await _service.Add(stockRequestTransactionToAdd);
            if (serviceResult.IsFailed)
            {
                return BackChannelResponseDto<StockRequestTransactionDto>.Failure(serviceResult.Error);
            }
            var resultDto = _mapper.Map<StockRequestTransaction, StockRequestTransactionDto>(serviceResult.Data);
            return BackChannelResponseDto<StockRequestTransactionDto>.Success(resultDto);
        }
    }
}
