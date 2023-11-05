using Azure;
using eShopAnalysis.PaymentAPI.Dto;
using eShopAnalysis.PaymentAPI.Models;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<object> AddPaymentTransactionAsync(AddPaymentTransactionRequestDto addPaymentTransactionRequest, IPaymentTransactionRepository transactionRepo)
        {
            if (transactionRepo is StripePaymentTransactionRepository transRepo) {
                string orderIdStr = string.Empty;
                bool haveOrderId = addPaymentTransactionRequest.StripePaymentIntentMeta.TryGetValue(_settings.Value.MetaOrderKey, out orderIdStr);

                if (haveOrderId == true) {
                    orderIdStr = addPaymentTransactionRequest.StripePaymentIntentMeta[_settings.Value.MetaOrderKey];
                }
                else { 
                    return null; 
                }

                var result = await transRepo.AddStripeTransactionAsync(
                    paymentIntentId: addPaymentTransactionRequest.StripePaymentIntentId, 
                    orderId: Guid.Parse(orderIdStr), 
                    customerId: addPaymentTransactionRequest.CustomerId, 
                    cardId: addPaymentTransactionRequest.StripeCardId, 
                    subTotal: Convert.ToDouble(addPaymentTransactionRequest.SubTotal),
                    discount: addPaymentTransactionRequest.Discount);

                return result;
            }
            else { throw new Exception("unknown error, please inspect more"); }
        }

        public bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> MakePaymentAsync(Guid userId, Guid orderId, double subTotal, double discount, string cardId, IUserCustomerMappingRepository mapping, IPaymentTransactionRepository paymentTransactionRepository)
        {
            StripeConfiguration.ApiKey = _settings.Value.SecretKey; //require no matter what
            if (mapping == null) { throw new ArgumentNullException("mapping"); }

            string customerId = mapping.GetCustomerIdOfUser(userId);
            if (customerId.IsNullOrEmpty())
            {
                var customerServiceOptions = new CustomerCreateOptions {
                    Description = "My First Test Customer (created for API docs at https://www.stripe.com/docs/api)",
                };
                var customerService = new CustomerService();
                var createdCustomer = await customerService.CreateAsync(customerServiceOptions);
                customerId = createdCustomer.Id;
                mapping.AddUserCustomerMapping(new UserCustomerMapping
                {
                    CustomerId = customerId,
                    UserId = userId,
                });
            } 
            else { //this is not the first time payment, then we check, but we did not check if this is paid in momo
                if (paymentTransactionRepository is not StripePaymentTransactionRepository transRepo) { 
                    throw new Exception("cannot resolve the type"); 
                }
                bool anyTransactionExistedForThisOrder = await transRepo.GetAsQueryable()
                                                                        .AsNoTracking()
                                                                        .Where(trans => trans.CustomerId == customerId)
                                                                        .AnyAsync(trans => trans.OrderId == orderId);
                //if the paymentTransaction is cancelled(refunded), we will still mark ti as Existed and will not create a checkout session for order with this ORDERID
                if (anyTransactionExistedForThisOrder) {
                    return null;
                }

            }


            //if cartId exist then call customerService to update default card(also retrive the list of card to validate if cardId is valid)(last 4 can be used to render on UI)
            if (!cardId.IsNullOrEmpty()) {
                CardService cardService = new ();
                CardListOptions cardListOptions = new () { Limit = 5 };
                var cards = await cardService.ListAsync(parentId: customerId, cardListOptions);

                bool cardIsValid = cards.Any(c => c.Id == cardId);
                if (cardIsValid) {
                    CustomerUpdateOptions customerUpdateOptions = new() { DefaultSource = cardId };
                    CustomerService customerService = new();
                    customerService.Update(customerId, customerUpdateOptions);
                }
            }

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
                { _settings.Value.MetaOrderKey , orderId.ToString() }
            };
            var sessionOptions = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = _settings.Value.SuccessUrl,
                CancelUrl = _settings.Value.CancelUrl,
                Customer = customerId.ToString(), //do not use mapping.GetCustomerIdOfUser(userId) because it still not saved to db
                Mode = "payment",
                PaymentMethodTypes = new List<string>() { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions {
                        UnitAmount = Convert.ToInt64(subTotal - discount),
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions { Name = "Order" + orderId },
                    }
                  },
                },
                //this session checkout will create and complete a payment intent, so i attact the meta to that payment intent, not the checkout session since we use webhook with payment intent succeed
                PaymentIntentData = new SessionPaymentIntentDataOptions() { Metadata = metaDict },
            };
            var sessionService = new SessionService();
            Session session = await sessionService.CreateAsync(sessionOptions);

            //another way to redirect in controller
            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);
            if (session == null)
                return null;
            return session.Url;

        }


    }
}
