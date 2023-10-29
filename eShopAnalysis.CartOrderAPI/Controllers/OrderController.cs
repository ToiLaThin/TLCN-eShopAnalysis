using eShopAnalysis.CartOrderAPI.Application.Commands;
using eShopAnalysis.CartOrderAPI.Application.Dto;
using eShopAnalysis.CartOrderAPI.Application.Queries;
using eShopAnalysis.CartOrderAPI.Application.Result;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.OrderAggregate;
using eShopAnalysis.CartOrderAPI.Services.BackchannelDto;
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
        public async Task<IEnumerable<OrderDraftViewModel>> GetDraftOrdersOfUser([FromQuery] string userId)
        {
            Guid userIdGuid = Guid.Parse(userId);
            var result = await _orderQueries.GetUserDraftOrders(userIdGuid);
            return result;
        }

        [HttpPost("ConfirmOrderCustomerInfo")]
        public async Task<Order> ConfirmOrderCustomerInfo([FromBody] CustomerOrderInfoConfirmedRequestDto customerOrderInfoConfirmedRequestDto)
        {
            //TODO for now i do thing minimun, please review it later
            OrderInfoConfirmCommand command = new OrderInfoConfirmCommand(customerOrderInfoConfirmedRequestDto.OrderId,customerOrderInfoConfirmedRequestDto.Address, customerOrderInfoConfirmedRequestDto.PhoneNumber);
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPut("PickPaymentMethodCOD")]
        public async Task<OrderViewModel> PickPaymentMethodCOD([FromQuery] Guid orderId)
        {
            PickPaymentMethodCODCommand command = new PickPaymentMethodCODCommand(orderId);
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPut("BackChannel/BulkApproveOrder")]
        public async Task<BackChannelResponseDto<IEnumerable<Guid>>> BulkApproveOrder([FromBody] IEnumerable<Guid> orderIdsToApprove)
        {
            if (orderIdsToApprove == null) { 
                throw new ArgumentNullException(nameof(orderIdsToApprove)); 
            }
            OrderApproveCommand command = new OrderApproveCommand(orderIdsToApprove);
            var result = await _mediator.Send(command);
            if (result == null) {
                return BackChannelResponseDto<IEnumerable<Guid>>.Failure("data have errors");
            }
            return BackChannelResponseDto<IEnumerable<Guid>>.Success(result);
        }

        //return minial orders with items quantity to aggregate api gateway
        //if swagger cannot get schema, view debug console to fix
        [HttpPost("BackChannel/GetToApprovedOrders")]
        public async Task<BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>> GetToApprovedOrders([FromBody] PagingOrderRequestDto pagingOrderRequest)
        {
            var result = await _orderQueries.GetToApprovedOrders(pagingOrderRequest.Limit);
            if (result == null)
            {
                return BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>.Failure("data have errors");
            }
            return BackChannelResponseDto<IEnumerable<OrderItemsResponseDto>>.Success(result);
        }

    }
}
