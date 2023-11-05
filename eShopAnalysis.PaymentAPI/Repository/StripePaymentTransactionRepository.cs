using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Models;
using Microsoft.EntityFrameworkCore;
using Stripe;

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
            await _context.StripeTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return await _context.StripeTransactions.FindAsync(paymentIntentId);
        }

        public StripeTransaction Get(string paymentIntentId)
        {
            var result = _context.StripeTransactions.Find(paymentIntentId);
            return result;
        }
        public async Task<StripeTransaction> GetAsync(string paymentIntentId)
        {
            var result = await _context.StripeTransactions.FindAsync(paymentIntentId);
            return result;
        }

        //https://stackoverflow.com/a/26677047
        //we should not return all by calling ToListAsync(even async), but on the service , we will add some filtering(where) and then call to list async
        public IQueryable<StripeTransaction> GetAsQueryable()
        {
            var result = _context.StripeTransactions.AsQueryable();
            return result;
        }
    }
}
