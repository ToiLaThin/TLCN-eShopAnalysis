using eShopAnalysis.PaymentAPI.Models;

namespace eShopAnalysis.PaymentAPI.Repository
{
    public interface IUserCustomerMappingRepository
    {
        string GetCustomerIdOfUser(Guid userId);

        UserCustomerMapping AddUserCustomerMapping(UserCustomerMapping mappingToAdd);

        UserCustomerMapping DeleteUserCustomerMapping(Guid userId);

        Task<string> GetCustomerIdOfUserAsync(Guid userId);

        Task<UserCustomerMapping> AddUserCustomerMappingAsync(UserCustomerMapping mappingToAdd);

        Task<UserCustomerMapping> DeleteUserCustomerMappingAsync(Guid userId);
    }
}
