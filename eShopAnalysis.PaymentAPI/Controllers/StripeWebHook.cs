using eShopAnalysis.PaymentAPI.Dto;
using eShopAnalysis.PaymentAPI.Service;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.Utilities;
using eShopAnalysis.PaymentAPI.Utilities.Behaviors;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LoggingBehaviorActionFilter))]
        public async Task<IActionResult> PaymentCompleted()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            if (String.IsNullOrEmpty(json)) {
                return Ok("Xui ghe");
            }
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
            //TODO since we must only store the amount received = subTotal - discount in the payment intent(review make payment process in the strategy), we have two option
            //1: change db to only store amount received only not subtotal and discount
            //2: store the discount amount and paid amount on the meta
            //the  paymentIntent.AmountReceived will actually store in the subTotal column of the StripePaymentTransaction table
            var addPaymentTransactionReq = AddPaymentTransactionRequestDto.CreateStripeInstance(paymentIntent.Id, paymentIntent.Metadata, paymentIntent.PaymentMethodId, paymentIntent.CustomerId, paymentIntent.AmountReceived, 0);
            var result = await _paymentService.AddPaymentTransactionAsync(addPaymentTransactionReq);
            return result;
        }
    }
}
