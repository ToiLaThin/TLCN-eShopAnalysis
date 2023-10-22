using eShopAnalysis.PaymentAPI.Models;

namespace eShopAnalysis.PaymentAPI.Repository
{
    public interface IPaymentTransactionRepository
    {
        //if we use generic, it will be bad for flexibility => so this is just a marker interface, we can still cast instance of this to MomoPaymentTransactionRepository or StripePaymentTransactionRepository (boxing, unboxing)
    }

    public interface IStripePaymentTransactionRepository: IPaymentTransactionRepository 
    {
        //this should be async (and have Async in it name), because even if the parent method(lower in stack trace) is async, if this is synchrous, it could still lock the thread
        Task<StripeTransaction?> AddStripeTransactionAsync(string paymentIntentId, Guid orderId, string customerId, string cardId, double subTotal, double discount = 0);
    }
    public interface IMomoPaymentTransactionRepository : IPaymentTransactionRepository 
    {
        MomoTransaction AddMomoTransaction(Guid orderId, string customerId, double subTotal, double discount = 0);
    }
}
