using eShopAnalysis.PaymentAPI.Repository;
using eShopAnalysis.PaymentAPI.UnitOfWork;

namespace eShopAnalysis.PaymentAPI.Service.Strategy
{
    public interface IPaymentStrategy
    {
        string? MakePayment(Guid userId, Guid orderId, double amount, IUserCustomerMappingRepository mapping);

        bool CancelPayment(Guid userId, Guid orderId, IUserCustomerMappingRepository mapping);
    }
}
