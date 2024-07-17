using AutoMapper;
using eShopAnalysis.CartOrderAPI.Application.Commands;
using eShopAnalysis.CartOrderAPI.Application.Dto;
using eShopAnalysis.CartOrderAPI.Application.Envelope;
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
        private IMapper _mapper;
        public OrderController(IMediator mediator, IOrderQueries orderQueries, IMapper mapper) {
            _mediator = mediator;
            _orderQueries = orderQueries;
            _mapper = mapper;
        }

        [HttpGet("GetDraftOrdersOfUser")]
        [ProducesResponseType(typeof(IEnumerable<OrderDraftViewModel>), StatusCodes.Status200OK)]
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


        //return type is now envelope
        [HttpGet("GetOrdersAggregateCartFilterSortPaginationOfUser")]
        [ProducesResponseType(typeof(OrderAggregateCartViewModelListEnvelope), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderAggregateCartViewModelListEnvelope>> GetOrdersAggregateCartFilterSortPaginationOfUser([FromQuery] string userId, [FromQuery] OrderStatus? filterOrderStatus,
            PaymentMethod? filterPaymentMethod,
            OrdersSortBy sortBy = OrdersSortBy.Id,
            int page = 1,
            int pageSize = 10,
            OrdersSortType sortType = OrdersSortType.Ascending)
        {
            Guid userIdGuid = Guid.Parse(userId);
            //enum default value is 0, if not passing value in, it have 0 as default value
            int filterPaymentMethodPassIn = filterPaymentMethod.HasValue ? (int)filterPaymentMethod : -1;
            int filterOrderStatusPassIn = filterOrderStatus.HasValue ? (int)filterOrderStatus : -1;
            var queryResult = await _orderQueries.GetOrdersAggregateCartFilterSortPaginationOfUser(
                    userIdGuid,
                    (OrderStatus)filterOrderStatusPassIn,
                    (PaymentMethod)filterPaymentMethodPassIn,
                    sortBy,
                    page,
                    pageSize,
                    sortType);

            if (queryResult.IsFailed || queryResult.IsException) {
                return NotFound(queryResult.Error);
            }

            //another query to get the total count
            int totalOrdersAfterFilter = await _orderQueries.GetOrdersAggregateCartTotalCountAfterFileteredOfUser((OrderStatus)filterOrderStatusPassIn, (PaymentMethod)filterPaymentMethodPassIn, userIdGuid);
            if ((totalOrdersAfterFilter == 0 && queryResult.Data.Count() > 0) || totalOrdersAfterFilter < queryResult.Data.Count()) {
                return NotFound("the total is not valid");
            }
            OrderAggregateCartViewModelListEnvelope envelope = new OrderAggregateCartViewModelListEnvelope(queryResult.Data, totalOrdersAfterFilter);
            return Ok(envelope);
        }

        [HttpGet("GetOrderAggregateCartByCartId")]
        [ProducesResponseType(typeof(OrderAggregateCartViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderAggregateCartViewModel>> GetOrderAggregateCartByCartId([FromQuery] string cartId)
        {
            Guid cartIdGuid = Guid.Parse(cartId);
            var queryResult = await _orderQueries.GetOrderAggregateCartByCartIdUsingRelationship(cartIdGuid);
            if (queryResult.IsFailed || queryResult.IsException) {
                return NotFound(queryResult.Error);
            }
            return Ok(queryResult.Data);
        }

        [HttpGet("GetOrderAggregateCartByOrderId")]
        [ProducesResponseType(typeof(OrderAggregateCartViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderAggregateCartViewModel>> GetOrderAggregateCartByOrderId([FromQuery] string orderId)
        {
            Guid orderIdGuid = Guid.Parse(orderId);
            var queryResult = await _orderQueries.GetOrderAggregateCartByOrderIdUsingRelationship(orderIdGuid);
            if (queryResult.IsFailed || queryResult.IsException) {
                return NotFound(queryResult.Error);
            }
            return Ok(queryResult.Data);
        }

        [HttpGet("GetOrdersToDeliver")]
        [ProducesResponseType(typeof(IEnumerable<OrderAggregateCartViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<IEnumerable<OrderAggregateCartViewModel>>> GetOrdersToDeliver()
        {
            var queryResult = await _orderQueries.GetOrdersToDeliver();
            if (queryResult.IsFailed || queryResult.IsException)
            {
                return NotFound(queryResult.Error);
            }
            return Ok(queryResult.Data);
        }

        [HttpPost("ConfirmOrderCustomerInfo")]
        [ProducesResponseType(typeof(OrderAggregateCartViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderAggregateCartViewModel>> ConfirmOrderCustomerInfo([FromBody] CustomerOrderInfoConfirmedRequestDto customerOrderInfoConfirmedRequestDto)
        {
            //TODO for now i do thing minimun, please review it later
            OrderInfoConfirmCommand command = new OrderInfoConfirmCommand(customerOrderInfoConfirmedRequestDto.OrderId, customerOrderInfoConfirmedRequestDto.Address, customerOrderInfoConfirmedRequestDto.PhoneNumber);
            var commandResult = await _mediator.Send(command);
            if (commandResult.IsFailed || commandResult.IsException) {
                return NotFound(commandResult.Error);
            }
            //Mapping from Order to OrderAggregateCartModel
            OrderAggregateCartViewModel mappingResult = _mapper.Map<Order, OrderAggregateCartViewModel>(commandResult.Data);
            return Ok(mappingResult);
        }

        [HttpPut("PickPaymentMethodCOD")]
        [ProducesResponseType(typeof(OrderAggregateCartViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<ActionResult<OrderAggregateCartViewModel>> PickPaymentMethodCOD([FromQuery] Guid orderId)
        {
            PickPaymentMethodCODCommand command = new PickPaymentMethodCODCommand(orderId);
            var commandResult = await _mediator.Send(command);
            if (commandResult.IsFailed || commandResult.IsException) {
                return NotFound(commandResult.Error);
            }
            //Mapping from Order to OrderAggregateCartModel
            OrderAggregateCartViewModel mappingResult = _mapper.Map<Order, OrderAggregateCartViewModel>(commandResult.Data);
            return Ok(mappingResult);
        }

        [HttpPost("BackChannel/GetOrderAggregateCartByCartId")]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<BackChannelResponseDto<OrderAggregateCartViewModel>> GetOrderAggregateCartByCartId([FromBody] ReferenceTypeWrapperDto<Guid> wrapperCartId)
        {
            if (wrapperCartId == null) {
                throw new ArgumentNullException(nameof(wrapperCartId));
            }
            if (wrapperCartId.Data == null) {
                throw new ArgumentNullException(nameof(wrapperCartId.Data));
            }
            Guid cartId = wrapperCartId.Data;
            //after the cart and order draft just create, no address for that order existed, so do not use GetOrderAggregateCartByCartIdUsingRelationship
            var queryResult = await _orderQueries.GetOrderAggregateCartByCartIdWithoutAddressUsingRelationship(cartId);
            if (queryResult.IsFailed || queryResult.IsException) {
                return BackChannelResponseDto<OrderAggregateCartViewModel>.Failure(queryResult.Error);
            }
            return BackChannelResponseDto<OrderAggregateCartViewModel>.Success(queryResult.Data);
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
