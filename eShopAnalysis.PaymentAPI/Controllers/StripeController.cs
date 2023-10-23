using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using eShopAnalysis.PaymentAPI.Dto;

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
        public async Task<PaymentResponseDto> MakePayment(PaymentRequestDto paymentRequest)
        {
            if (paymentRequest == null) { throw new ArgumentNullException(nameof(paymentRequest)); }
            var payUrl = await _paymentService.MakePayment(userId: paymentRequest.UserId,
                                                           orderId: paymentRequest.OrderId,
                                                           subTotal: paymentRequest.SubTotal,
                                                           discount: paymentRequest.TotalDiscount,
                                                           cardId: paymentRequest.CardId);

            if (payUrl.IsNullOrEmpty()) { 
                return new PaymentResponseDto() { PayUrl = String.Empty }; 
            }
            return new PaymentResponseDto() { PayUrl = payUrl };
        }

    }
}
