using eShopAnalysis.CartOrderAPI.Application.Commands;
using eShopAnalysis.CartOrderAPI.Domain.DomainModels.CartAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShopAnalysis.CartOrderAPI.Controllers
{
    [Route("api/OrderCartAPI/CartAPI")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private IMediator _mediator;
        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddCart")]
        //only have one thing can be in FromBody
        //also in get request should not have body
        //TODO create a CartCreateRequestDTO have both cartItems and userId since only one model can reside in body
        // if CartCreateCommand have private setter, it will not have example schema in the swagger index.html and the data we received (cartItem and userId) will be null and default Guid
        //so in controller we received a Dto and create the command
        //TODO we could also use factory to create command with validation the input
        public async Task<CartSummary> AddCart([FromBody] IEnumerable<CartItem> cartItems,[FromHeader] Guid userId) { 
            CartCreateCommand command = new CartCreateCommand(cartItems, userId);
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
