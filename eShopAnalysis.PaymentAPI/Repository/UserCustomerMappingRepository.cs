using eShopAnalysis.PaymentAPI.Data;
using eShopAnalysis.PaymentAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.PaymentAPI.Repository
{
    public class UserCustomerMappingRepository : IUserCustomerMappingRepository
    {
        private readonly PaymentContext _paymentContext;

        public UserCustomerMappingRepository(PaymentContext paymentContext) { _paymentContext = paymentContext; }

        public UserCustomerMapping AddUserCustomerMapping(UserCustomerMapping mappingToAdd)
        {
            _paymentContext.Add(mappingToAdd);
            try {
                _paymentContext.SaveChanges();
                return mappingToAdd;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<UserCustomerMapping> AddUserCustomerMappingAsync(UserCustomerMapping mappingToAdd)
        {
            await _paymentContext.AddAsync(mappingToAdd);
            try {
                await _paymentContext.SaveChangesAsync();
                return mappingToAdd;
            }
            catch (Exception ex) { 
                throw ex; 
            }
        }

        public UserCustomerMapping DeleteUserCustomerMapping(Guid userId)
        {
            var mappingToDel = _paymentContext.UserCustomerMappings.Find(userId);
            if (mappingToDel == null) {
                throw new Exception("mapping does not exist");
                //return null;
            }

            try {
                _paymentContext.Remove(mappingToDel);
                _paymentContext.SaveChanges();
                return mappingToDel;
            }
            catch { return null; }
        }

        public async Task<UserCustomerMapping> DeleteUserCustomerMappingAsync(Guid userId)
        {
            var mappingToDel = await _paymentContext.UserCustomerMappings.FindAsync(userId);
            if (mappingToDel == null) {
                throw new Exception("mapping does not exist");
                //return null;
            }

            try {
                _paymentContext.Remove(mappingToDel);
                await _paymentContext.SaveChangesAsync();
                return mappingToDel;
            }
            catch { 
                return null; 
            }
        }

        public string GetCustomerIdOfUser(Guid userId)
        {
            var mapping = _paymentContext.UserCustomerMappings.AsNoTracking()
                                                              .Where(map => map.UserId.Equals(userId))
                                                              .Select(m => m.CustomerId)
                                                              .ToList();
            if (mapping == null) {
                return null;
                throw new Exception("not exist");
            }
            if (mapping.Count > 1) {
                return null;
                throw new Exception("one user contain many mapping?");
            }
            if (mapping.Count <= 0)
            {
                return null;
                throw new Exception("no mapping");
            }
            //must be exact 1
            return mapping[0];
        }

        public async Task<string> GetCustomerIdOfUserAsync(Guid userId)
        {
            var mapping = await _paymentContext.UserCustomerMappings.AsNoTracking()
                                                                    .Where(map => map.UserId.Equals(userId))
                                                                    .Select(m => m.CustomerId)
                                                                    .ToListAsync();
            if (mapping == null) {
                return null;
                throw new Exception("not exist");
            }
            if (mapping.Count > 1) {
                return null;
                throw new Exception("one user contain many mapping?");
            }
            if (mapping.Count <= 0) {
                return null;
                throw new Exception("no mapping");
            }
            //must be exact 1
            return mapping[0];
        }
    }
}
