using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Service.Strategy
{
    public interface IPaymentStrategy
    {
        string? MakePayment(Guid userId, Guid orderId, double subTotal, double discount, string cardId, IUserCustomerMappingRepository mapping);

        bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping);

        Task<object> AddPaymentTransactionAsync(PaymentIntent infoObj, IPaymentTransactionRepository transactionRepo);
    }
}
