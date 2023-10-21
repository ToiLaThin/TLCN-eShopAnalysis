using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace eShopAnalysis.PaymentAPI.Controllers
{
    [Route("api/PaymentAPI/StripeAPI")]
    [ApiController]
    public class StripeController : ControllerBase
    {
        private IPaymentService<StripePaymentStrategy> _paymentService;
        public StripeController(IPaymentService<StripePaymentStrategy> paymentService)
        {
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
