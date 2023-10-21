using eShopAnalysis.PaymentAPI.Models;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using eShopAnalysis.PaymentAPI.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

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

        public bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping)
        {
            throw new NotImplementedException();
        }

        public string? MakePayment(Guid userId, Guid orderId, double amount, IUserCustomerMappingRepository mapping)
        {
            //kiem tra da co user mapping chua, neu chua thi phai tao
            if (userId == null ) {  throw new ArgumentNullException("userId"); }
            if (orderId == null) { throw new ArgumentNullException("orderId"); }
            if (amount == null) { throw new ArgumentNullException("amount"); }
            if (mapping == null) { throw new ArgumentNullException("uOW"); }

            string? checkCustomerId = mapping.GetCustomerIdOfUser(userId);
            //neu chua co thi create mapping 
            if (checkCustomerId.IsNullOrEmpty()) {
                string newCustomerId = "cus_testABCD12345";
                mapping.AddUserCustomerMapping(new UserCustomerMapping { 
                    CustomerId = newCustomerId, UserId = userId 
                });
            }
            //the method to send a request to momo
            string responseFromMomo = this.PrepareAndSendPaymentRequest(orderId, amount);

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
            string partnerCode = "MOMOI0LX20220922";
            string accessKey = "5DuZliGMfIjEIiQs";
            string serectkey = "JREEY5yK0azGS6YJZV5LQXB2wYR9SN8J";
            string orderIdStr = orderId.ToString();
            string orderInfo = $"Order {orderIdStr}";
            string redirectUrl = "http://localhost:4200/";
            string ipnUrl = "http://localhost:4200/";
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
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
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
