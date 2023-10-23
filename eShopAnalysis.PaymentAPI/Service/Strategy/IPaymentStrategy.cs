using eShopAnalysis.PaymentAPI.Dto;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Service.Strategy
{
    public interface IPaymentStrategy
    {
        Task<string?> MakePaymentAsync(Guid userId, Guid orderId, double subTotal, double discount, string cardId, IUserCustomerMappingRepository mapping, IPaymentTransactionRepository paymentTransactionRepository);

        bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping);

        Task<object> AddPaymentTransactionAsync(AddPaymentTransactionRequestDto addPaymentTransactionRequest, IPaymentTransactionRepository transactionRepo);
    }
}
