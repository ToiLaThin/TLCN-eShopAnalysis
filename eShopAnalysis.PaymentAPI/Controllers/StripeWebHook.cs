using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Controllers
{
    [Route("api/PaymentAPI/StripeAPI/WebHook")]
    [ApiController]
    public class StripeWebHook: ControllerBase
    {
        [HttpPost("PaymentSucceed")]
        public async Task<IActionResult> PaymentCompleted()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try {
                var stripeEvent = EventUtility.ParseEvent(json);
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded) {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent);
                    return Ok("web hook received it");
                }
                return BadRequest();
            }
            catch (StripeException e) {
                return BadRequest();
            }
        }
    }
}
