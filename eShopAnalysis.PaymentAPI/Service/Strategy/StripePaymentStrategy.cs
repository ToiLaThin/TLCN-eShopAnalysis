using Azure;
using eShopAnalysis.PaymentAPI.Models;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;

namespace eShopAnalysis.PaymentAPI.Service.Strategy
{
    public class StripePaymentStrategy : IPaymentStrategy
    {
        IOptions<StripeSetting> _settings;
        public StripePaymentStrategy(IOptions<StripeSetting> settings) { 
            _settings = settings;
        }
        public bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping)
        {
            throw new NotImplementedException();
        }

        public string? MakePayment(Guid userId, Guid orderId, double amount, IUserCustomerMappingRepository mapping)
        {
            StripeConfiguration.ApiKey = _settings.Value.SecretKey; //require no matter what
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (orderId == null) { throw new ArgumentNullException("orderId"); }
            if (amount == null) { throw new ArgumentNullException("amount"); }
            if (mapping == null) { throw new ArgumentNullException("uOW"); }

            string customerId = mapping.GetCustomerIdOfUser(userId);
            //neu chua co thi create mapping 
            if (customerId.IsNullOrEmpty())
            {
                var customerServiceOptions = new CustomerCreateOptions {
                    Description = "My First Test Customer (created for API docs at https://www.stripe.com/docs/api)",
                };
                var customerService = new CustomerService();
                var createdCustomer = customerService.Create(customerServiceOptions);
                customerId = createdCustomer.Id;
                mapping.AddUserCustomerMapping(new UserCustomerMapping
                {
                    CustomerId = customerId,
                    UserId = userId,
                });
            }
            //if cartId then call customerService to update default card
            //CustomerUpdateOptions customerUpdateOptions = new()
            //{
            //    DefaultSource = cardId
            //}

            //programatically register webhook endpoint
            //phai public access dc, hoac co the local nhung phai dung stripe CLI stripe listen --forward-to localhost:7001/api/Payment/StripeAPI/WebHook/PaymentSucceed
            //var options = new WebhookEndpointCreateOptions
            //{
            //    Url = "http://localhost:7001/api/Payment/StripeAPI/WebHook/PaymentSucceed",
            //    EnabledEvents = new List<String> {
            //        //"payment_intent.payment_failed",
            //        "payment_intent.succeeded",
            //    },
            //};
            //var service = new WebhookEndpointService();
            //var endpoint = service.Create(options);

            var metaDict = new Dictionary<string, string>
            {
                { "orderId", orderId.ToString() }
            };
            var sessionOptions = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = _settings.Value.SuccessUrl,
                CancelUrl = _settings.Value.CancelUrl,
                Customer = customerId.ToString(), //do not use mapping.GetCustomerIdOfUser(userId) because it still not saved to db
                Mode = "payment",
                PaymentMethodTypes = new List<string>() { "card" },
                Metadata = metaDict,
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions {
                        UnitAmountDecimal = Convert.ToDecimal(amount),
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Order" + orderId }
                    }
                  },
                },
            };
            var sessionService = new SessionService();
            Session session = sessionService.Create(sessionOptions);

            //another way to redirect in controller
            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);
            if (session == null)
                return null;
            return session.Url;

        }
    }
}
