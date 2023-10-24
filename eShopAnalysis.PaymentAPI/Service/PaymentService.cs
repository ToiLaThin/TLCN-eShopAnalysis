using eShopAnalysis.EventBus.Abstraction;
using eShopAnalysis.PaymentAPI.Dto;
using eShopAnalysis.PaymentAPI.IntegrationEvents;
using eShopAnalysis.PaymentAPI.Models;
using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.Service.Strategy;
using eShopAnalysis.PaymentAPI.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using PaymentMethod = eShopAnalysis.PaymentAPI.Models.PaymentMethod;

namespace eShopAnalysis.PaymentAPI.Service
{
    //the controller will specify which strategy to take
    public class PaymentService<PS>: IPaymentService<PS> where PS: IPaymentStrategy
    {
        private IUnitOfWork _uOW;
        private IPaymentStrategy _paymentStrategy;
        private IEventBus _eventBus;

        public PaymentService(IUnitOfWork uOW, IPaymentStrategy paymentStrategy, IEventBus eventBus) { 
            _uOW = uOW;
            _paymentStrategy = paymentStrategy;
            _eventBus = eventBus;
        }

        //the adding of the momoTransaction will be handle in the IPN(instand payment notification)
        //cardId is optional, user can choose one of their card or input at the stripe hosted page
        //TODO, add validation for card(Done and checked with swagger) 
        public async Task<string?> MakePayment(Guid userId, Guid orderId, double subTotal, double discount = 0, string cardId = "")
        {
            if (userId == null) { throw new ArgumentNullException("userId"); }
            if (orderId == null) { throw new ArgumentNullException("orderId"); }
            if (subTotal == null || subTotal <= 0) { throw new ArgumentException("subTotal"); }
            if (discount < 0) { throw new ArgumentException("discount"); }

            var transaction = await _uOW.BeginTransactionAsync();

            IPaymentTransactionRepository paymentTransactionRepoToValidatePayment;
            if (_paymentStrategy is MomoPaymentStrategy) {
                paymentTransactionRepoToValidatePayment = _uOW.MomoPaymentTransactionRepository;
            } else if (_paymentStrategy is StripePaymentStrategy) {
                paymentTransactionRepoToValidatePayment = _uOW.StripePaymentTransactionRepository;
            } else { throw  new Exception("unknown payment strategy or cannot parse the type"); }
            string? paymentRedirectUrl = await _paymentStrategy.MakePaymentAsync(userId, orderId, subTotal, discount, cardId, _uOW.UserCustomerMappingRepository, paymentTransactionRepoToValidatePayment);
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

        public async Task<object?> AddPaymentTransactionAsync(AddPaymentTransactionRequestDto addPaymentTransactionRequest)
        {
            if (addPaymentTransactionRequest == null) { throw new ArgumentNullException(nameof(addPaymentTransactionRequest)); }
            //await this task
            var transaction = await _uOW.BeginTransactionAsync();
            if (_paymentStrategy is MomoPaymentStrategy) {
                var result = _paymentStrategy.AddPaymentTransactionAsync(addPaymentTransactionRequest, _uOW.MomoPaymentTransactionRepository);
                if (result == null) {
                    _uOW.RollbackTransaction();
                }
                await _uOW.CommitTransactionAsync(transaction);
                return result;
            } else if (_paymentStrategy is StripePaymentStrategy) {
                var result = await _paymentStrategy.AddPaymentTransactionAsync(addPaymentTransactionRequest, _uOW.StripePaymentTransactionRepository);
                if (result == null)
                {
                    _uOW.RollbackTransaction();
                }
                await _uOW.CommitTransactionAsync(transaction);
                var stripeTransaction = result as StripeTransaction;
                if (stripeTransaction == null) { 
                    throw new Exception("Cannot parse to stripe transaction"); 
                }
                _eventBus.Publish(
                    new OrderPaymentTransactionCompletedIntegrationEvent(
                        orderId: stripeTransaction.OrderId,
                        paymentMethod: PaymentMethod.CreditCard
                    )
                );
                return result;
            } else { throw new Exception("some unclear error, please inspect more"); }

        }
    }
}
