using eShopAnalysis.PaymentAPI.Dto;
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
        public async Task<PaymentResponseDto> MakePayment(PaymentRequestDto paymentRequest)
        {
            if (paymentRequest == null) { throw new ArgumentNullException(nameof(paymentRequest)); }
            var payUrl = await _paymentService.MakePayment(userId: paymentRequest.UserId,
                                                           orderId: paymentRequest.OrderId,
                                                           subTotal: paymentRequest.SubTotal,
                                                           discount: paymentRequest.TotalDiscount);

            if (payUrl.IsNullOrEmpty())
            {
                return new PaymentResponseDto() { PayUrl = String.Empty };
            }
            return new PaymentResponseDto() { PayUrl = payUrl };
        }
    }
}
