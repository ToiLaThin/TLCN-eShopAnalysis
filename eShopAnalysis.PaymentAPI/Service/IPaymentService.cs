using eShopAnalysis.PaymentAPI.Service.Strategy;

namespace eShopAnalysis.PaymentAPI.Service
{
    public interface IPaymentService<PS> where PS: IPaymentStrategy
    {
        Task<bool> CancelPayment(Guid userId, Guid orderId);
        Task<string?> MakePayment(Guid userId, Guid orderId, double amount);
    }
}