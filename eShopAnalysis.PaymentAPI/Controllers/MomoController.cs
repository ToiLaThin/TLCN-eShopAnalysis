using eShopAnalysis.PaymentAPI.Service;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eShopAnalysis.PaymentAPI.Controllers
{
    //api controller can return Redirect to a url
    //TODO convert to generic IPaymentService with a composite of IPaymentStrategy, then in the constructor, try to resolve Concrete type of IPaymentStrategy using reflection(or any other method)
    [Route("api/PaymentAPI/MomoAPI")]
    [ApiController]
    public class MomoController : ControllerBase
    {
        private IPaymentService<MomoPaymentStrategy> _paymentService;
        public MomoController(IPaymentService<MomoPaymentStrategy> paymentService) {
            _paymentService = paymentService;
        }
        [HttpPost("MakePayment")] 
        public async Task<IActionResult> MakePayment(Guid userId, Guid orderId)
        {
            var payUrl = await _paymentService.MakePayment(userId: userId, orderId: orderId, amount: 1000);

            if (payUrl.IsNullOrEmpty()) { return BadRequest("try again"); }
            return Redirect(payUrl);
        }
    }
}
