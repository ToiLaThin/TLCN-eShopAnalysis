using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Models;

namespace eShopAnalysis.PaymentAPI.Repository
{
    public class StripePaymentTransactionRepository : IStripePaymentTransactionRepository
    {
        private readonly PaymentContext _context;
        public StripePaymentTransactionRepository(PaymentContext context) { _context = context; }

        //this should be async (and have Async in it name), because even if the parent method(lower in stack trace) is async, if this is synchrous, it could still lock the thread
        public async Task<StripeTransaction?> AddStripeTransactionAsync(string paymentIntentId, Guid orderId, string customerId, string cardId, double subTotal, double discount = 0)
        {
            //handle idempotency http request, if already exist, do nothing and return null meaning no new transaction added
            //use FindAsync, after the debuger jump from this, it return OK on the controller, then come back here, if we use Find here , it is still locking the thread(tested)
            StripeTransaction existingStripeTransaction = await _context.StripeTransactions.FindAsync(paymentIntentId);
            if (existingStripeTransaction != null) { return null; }

            StripeTransaction transaction = new StripeTransaction
            {
                PaymentIntentId = paymentIntentId,
                CustomerId = customerId,
                OrderId = orderId,
                CardId = cardId,
                SubTotal = subTotal,
                TotalDiscount = discount,
                TransactionStatus = PaymentStatus.Pending
            };
            _context.StripeTransactions.Add(transaction);
            _context.SaveChanges();
            return await _context.StripeTransactions.FindAsync(paymentIntentId);
        }
    }
}
