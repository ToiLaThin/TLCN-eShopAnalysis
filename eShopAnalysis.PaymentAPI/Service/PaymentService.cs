using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<string?> MakePayment(Guid userId, Guid orderId, double amount)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            string? paymentRedirectUrl = _paymentStrategy.MakePayment(userId, orderId, amount, _uOW.UserCustomerMappingRepository);
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
    }
}
