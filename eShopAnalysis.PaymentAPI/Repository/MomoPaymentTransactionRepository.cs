using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Models;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Repository
{
    public class MomoPaymentTransactionRepository : IMomoPaymentTransactionRepository
    {
        private readonly PaymentContext _context;
        public MomoPaymentTransactionRepository(PaymentContext context) { _context = context; }
        public MomoTransaction AddMomoTransaction(Guid orderId, string customerId, double subTotal, double discount = 0)
        {
            //handle idempotency http request, if already exist, do nothing and return null meaning no new transaction added
            StripeTransaction existingMomoTransaction = _context.StripeTransactions.Find(orderId);
            if (existingMomoTransaction != null) { return null; }

            MomoTransaction transaction = new MomoTransaction
            {
                TransactionId = orderId,
                CustomerId = customerId,
                SubTotal = subTotal,
                TotalDiscount = discount,
                TransactionStatus = PaymentStatus.Pending
            };
            _context.MomoTransactions.Add(transaction);
            _context.SaveChanges();
            return _context.MomoTransactions.Find(orderId);
        }
    }
}
