using eShopAnalysis.PaymentAPI.Models;

namespace eShopAnalysis.PaymentAPI.Repository
{
    public interface IUserCustomerMappingRepository
    {
        string GetCustomerIdOfUser(Guid userId);

        UserCustomerMapping AddUserCustomerMapping(UserCustomerMapping mappingToAdd);

        UserCustomerMapping DeleteUserCustomerMapping(Guid userId);
    }
}
