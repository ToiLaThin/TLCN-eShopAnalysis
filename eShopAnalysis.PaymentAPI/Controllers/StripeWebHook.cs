using eShopAnalysis.PaymentAPI.Service;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace eShopAnalysis.PaymentAPI.Controllers
{
    [Route("api/PaymentAPI/StripeAPI/WebHook")]
    [ApiController]
    public class StripeWebHook: ControllerBase
    {
        private IPaymentService<StripePaymentStrategy> _paymentService;
        private IOptions<StripeSetting> _settings;
        public StripeWebHook(IPaymentService<StripePaymentStrategy> paymentService, IOptions<StripeSetting> settings) { _paymentService = paymentService; _settings = settings; }

        //Handles POST requests with a JSON payload consisting of an event object.
        //Quickly returns a successful status code (2xx) prior to any complex logic that could cause a timeout.For example, you must return a 200 response before updating a customer’s invoice as paid in your accounting system.
        //THE HANDLE MUST BE ASYNCHRONOUS WITHOUT AN AWAIT
        [HttpPost("PaymentSucceed")]
        public async Task<IActionResult> PaymentCompleted()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try {
                var stripeEvent = EventUtility.ParseEvent(json);
                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentSucceeded) {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Then define and call a method to handle the successful payment intent.(without the await keyword)
                    // And also notice to make sure in the stack call of this method, (especially the top of the stack | final method to be call), no synchronous operation, no thread-blocking property
                    https://stackoverflow.com/questions/27265818/does-the-use-of-async-await-create-a-new-thread and https://www.pluralsight.com/guides/understand-control-flow-async-await
                    var result = await HandlePaymentIntentSucceedAsync(paymentIntent); //if you do not have await, will have exception UnitOfWork dispose but there still a data reader connection => cause the data not save in the db
                    //_ = HandlePaymentIntentSucceedAsync(paymentIntent); _ to ignore the result

                    // Another option might be create a new thread | Task.Run the method

                    if (result != null)
                        return Ok("web hook received charge payment intent succeed");
                    return BadRequest("Could not add transaction");
                }
                return NoContent();
            }
            catch (StripeException e) {
                return BadRequest();
            }
        }
        private async Task<object> HandlePaymentIntentSucceedAsync(PaymentIntent paymentIntent)
        {
            var result = await _paymentService.AddPaymentTransactionAsync(paymentIntent);
            return result;
        }
    }
}
