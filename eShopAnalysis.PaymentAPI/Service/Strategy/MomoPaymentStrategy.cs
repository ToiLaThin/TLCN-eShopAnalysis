using eShopAnalysis.PaymentAPI.Models;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Service.Strategy
{
    public class MomoPaymentStrategy: IPaymentStrategy
    {
        private IOptions<MomoSetting> _setting;

        public MomoPaymentStrategy(IOptions<MomoSetting> settings)
        {
            //must be DI to have value, not by calling Options.Create(new MomoSetting())
            _setting = settings;
        }

        public Task<object> AddPaymentTransactionAsync(PaymentIntent infoObj, IPaymentTransactionRepository transactionRepo)
        {
            throw new NotImplementedException();
        }

        public bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping)
        {
            throw new NotImplementedException();
        }

        //in momo we won't use cardId, so we just pass it in and do nothing with it
        public string? MakePayment(Guid userId, Guid orderId, double subTotal, double discount, string cardId, IUserCustomerMappingRepository mapping)
        {
            //kiem tra da co user mapping chua, neu chua thi phai tao
            if (mapping == null) { throw new ArgumentNullException("uOW"); }

            string? checkCustomerId = mapping.GetCustomerIdOfUser(userId);
            //neu chua co thi create mapping 
            if (checkCustomerId.IsNullOrEmpty()) {
                //even if we pay with momo, still need to create stripe customer, so next time this customer, if pay by stripe will have a valid customer id
                var customerServiceOptions = new CustomerCreateOptions
                {
                    Description = "My First Test Customer (created for API docs at https://www.stripe.com/docs/api)",
                };
                var customerService = new CustomerService();
                var createdCustomer = customerService.Create(customerServiceOptions);
                mapping.AddUserCustomerMapping(new UserCustomerMapping { 
                    CustomerId = createdCustomer.Id, UserId = userId 
                });
            }
            //the method to send a request to momo
            string responseFromMomo = this.PrepareAndSendPaymentRequest(orderId, subTotal - discount);

            //if the request is completed successfylly, then return the payUrl and signal Service to commit the transaction
            JObject jmessage = JObject.Parse(responseFromMomo);
            bool didSuccessfullyOrNot = jmessage.GetValue("resultCode").ToString().Equals("0");
            if (didSuccessfullyOrNot == true) {
                return jmessage.GetValue("payUrl").ToString();
            }
            return null;
        }

        private string PrepareAndSendPaymentRequest(Guid orderId, double amount)
        {
            string endpoint = "https://payment.momo.vn/v2/gateway/api/create";
            string partnerCode = _setting.Value.PartnerCode;
            string accessKey = _setting.Value.AccessKey;
            string serectkey = _setting.Value.SecretKey;
            string orderIdStr = orderId.ToString();
            string orderInfo = $"Order {orderIdStr}";
            string redirectUrl = _setting.Value.PaymentSuccessRedirectUrl;
            string ipnUrl = _setting.Value.InstantPaymentNotificationUrl; //TODO change this in appsettings
            string requestType = "captureWallet";

            string amountStr = amount.ToString();
            string requestId = Guid.NewGuid().ToString(); //for idempotency control
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", _setting.Value.PartnerName },
                { "storeId", _setting.Value.StoreId },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
            string responseFromMomo = MomoPaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            return responseFromMomo;
        }
    }
}
