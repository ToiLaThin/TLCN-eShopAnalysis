using eShopAnalysis.CartOrderAPI.Application.Commands;
using eShopAnalysis.CartOrderAPI.Application.Dto;
using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;
using eShopAnalysis.CartOrderAPI.Utilities.Behaviors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eShopAnalysis.CartOrderAPI.Controllers
{
    [Route("api/OrderCartAPI/OrderAPI")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IMediator _mediator;
        private IOrderQueries _orderQueries;
        public OrderController(IMediator mediator, IOrderQueries orderQueries) {
            _mediator = mediator;
            _orderQueries = orderQueries;
        }

        [HttpGet("GetDraftOrdersOfUser")]
        [ProducesResponseType(typeof(CartSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<OrderDraftViewModel>>> GetDraftOrdersOfUser([FromQuery] string userId)
        {
            Guid userIdGuid = Guid.Parse(userId);
            var queryResult = await _orderQueries.GetUserDraftOrders(userIdGuid);
            if (queryResult.IsFailed || queryResult.IsException) {
                return NotFound(queryResult.Error);
            }
            return Ok(queryResult.Data);
        }

        [HttpPost("ConfirmOrderCustomerInfo")]
        [ProducesResponseType(typeof(CartSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<Order>> ConfirmOrderCustomerInfo([FromBody] CustomerOrderInfoConfirmedRequestDto customerOrderInfoConfirmedRequestDto)
        {
            //TODO for now i do thing minimun, please review it later
            OrderInfoConfirmCommand command = new OrderInfoConfirmCommand(customerOrderInfoConfirmedRequestDto.OrderId,customerOrderInfoConfirmedRequestDto.Address, customerOrderInfoConfirmedRequestDto.PhoneNumber);
            var commandResult = await _mediator.Send(command);
            if (commandResult.IsFailed || commandResult.IsException) {
                return NotFound(commandResult.Error);
            }
            return Ok(commandResult.Data);
        }

        [HttpPut("PickPaymentMethodCOD")]
        [ProducesResponseType(typeof(CartSummary), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderViewModel>> PickPaymentMethodCOD([FromQuery] Guid orderId)
        {
            PickPaymentMethodCODCommand command = new PickPaymentMethodCODCommand(orderId);
            var commandResult = await _mediator.Send(command);
            if (commandResult.IsFailed || commandResult.IsException) {
                return NotFound(commandResult.Error);
            }
            return Ok(commandResult.Data);
        }

        [HttpPut("BackChannel/BulkApproveOrder")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<Guid>>> BulkApproveOrder([FromBody] IEnumerable<Guid> orderIdsToApprove)
        {
            if (orderIdsToApprove == null) { 
                throw new ArgumentNullException(nameof(orderIdsToApprove)); 
            }
            OrderApproveCommand command = new OrderApproveCommand(orderIdsToApprove);
            var commandResult = await _mediator.Send(command);
            if (commandResult.IsFailed || commandResult.IsException) {
                return BackChannelResponseDto<IEnumerable<Guid>>.Failure(commandResult.Error);
            }
            return BackChannelResponseDto<IEnumerable<Guid>>.Success(commandResult.Data);
        }

        //return minial orders with items quantity to aggregate api gateway
        //if swagger cannot get schema, view debug console to fix
        [HttpPost("BackChannel/GetToApprovedOrders")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders([FromBody] PagingOrderRequestDto pagingOrderRequest)
        {
            var queryResult = await _orderQueries.GetToApprovedOrders(pagingOrderRequest.Limit);
            if (queryResult.IsFailed || queryResult.IsException) {
                return BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>.Failure("data have errors");
            }
            return BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>.Success(queryResult.Data);
        }

    }
}
