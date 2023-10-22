using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace eShopAnalysis.PaymentAPI.Service
{
    //the controller will specify which strategy to take
    public class PaymentService<PS>: IPaymentService<PS> where PS: IPaymentStrategy
    {
        private IUnitOfWork _uOW;
        private IPaymentStrategy _paymentStrategy;

        public PaymentService(IUnitOfWork uOW, IPaymentStrategy paymentStrategy) { 
            _uOW = uOW;
            _paymentStrategy = paymentStrategy;
        }

        //the adding of the momoTransaction will be handle in the IPN(instand payment notification)
        //cardId is optional, user can choose one of their card or input at the stripe hosted page
        //TODO, add validation for card 
        public async Task<string?> MakePayment(Guid userId, Guid orderId, double subTotal, double discount = 0, string cardId = "")
        {
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (orderId == null) { throw new ArgumentNullException("orderId"); }
            if (subTotal == null || subTotal <= 0) { throw new ArgumentException("subTotal"); }
            if (discount < 0) { throw new ArgumentException("discount"); }

            var transaction = await _uOW.BeginTransactionAsync();
            string? paymentRedirectUrl = _paymentStrategy.MakePayment(userId, orderId, subTotal, discount, cardId, _uOW.UserCustomerMappingRepository);
            if (!paymentRedirectUrl.IsNullOrEmpty()) 
            {
                _uOW.CommitTransactionAsync(transaction);
                return paymentRedirectUrl;
            }
            _uOW.RollbackTransaction();
            return null;
        }

        public async Task<bool> CancelPayment(Guid userId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<object?> AddPaymentTransactionAsync(PaymentIntent infoObj)
        {
            //await this task
            var transaction = await _uOW.BeginTransactionAsync();
            if (_paymentStrategy is MomoPaymentStrategy) {
                var result = _paymentStrategy.AddPaymentTransactionAsync(infoObj, _uOW.MomoPaymentTransactionRepository);
                if (result == null) {
                    _uOW.RollbackTransaction();
                }
                await _uOW.CommitTransactionAsync(transaction);
                return result;
            } else if (_paymentStrategy is StripePaymentStrategy) {
                var result = await _paymentStrategy.AddPaymentTransactionAsync(infoObj, _uOW.StripePaymentTransactionRepository);
                if (result == null)
                {
                    _uOW.RollbackTransaction();
                }
                await _uOW.CommitTransactionAsync(transaction);
                return result;
            } else { throw new Exception("some unclear error, please inspect more"); }

        }
    }
}
