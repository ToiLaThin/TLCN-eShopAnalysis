using eShopAnalysis.PaymentAPI.Service.Strategy;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Service
{
    public interface IPaymentService<PS> where PS: IPaymentStrategy
    {
        Task<bool> CancelPayment(Guid userId, Guid orderId);
        Task<string?> MakePayment(Guid userId, Guid orderId, double subTotal, double discount = 0, string cardId = "");

        //TODO change input PaymentIntent to a class more generic can hold both info for momo transaction and stripe transaction
        Task<object?> AddPaymentTransactionAsync(PaymentIntent infoObj);
    }
}