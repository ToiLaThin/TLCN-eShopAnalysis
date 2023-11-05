using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace eShopAnalysis.CouponSaleItemAPI.Service
{
    public class CouponService : ICouponService
    {
        private IUnitOfWork _uOW;        
        public CouponService(IUnitOfWork uOW)
        {
            _uOW = uOW;
        }

        public async Task<ServiceResponseDto<Coupon>> Add(Coupon coupon)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = await _uOW.CouponRepository.AddAsync(coupon);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<Coupon>.Failure("cannot add coupon, rolled back the transaction");
            }
            await transaction.CommitAsync(); //because return no value
            return ServiceResponseDto<Coupon>.Success(result);
        }

        public async Task<ServiceResponseDto<Coupon>> Delete(Guid coupon)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = await _uOW.CouponRepository.DeleteAsync(coupon);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<Coupon>.Failure("cannot delete coupon, rolled back the transaction");
            }
            await transaction.CommitAsync();
            return ServiceResponseDto<Coupon>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<Coupon>>> GetAll()
        {
            var result = await _uOW.CouponRepository.GetAsQueryable().AsNoTracking().ToListAsync();
            return ServiceResponseDto<IEnumerable<Coupon>>.Success(result);
        }

        public async Task<ServiceResponseDto<Coupon>> GetCoupon(Guid couponId)
        {
            var result = await _uOW.CouponRepository.GetAsync(couponId);
            return ServiceResponseDto<Coupon>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<Coupon>>> GetCouponUsedByUser(Guid userId)
        {
            var result = await _uOW.CouponUserRepository.GetAsQueryableIncludedCouponUsed()
                                                  .AsNoTracking()
                                                  .Where(x => x.UserId == userId)
                                                  .Select(c => c.CouponUsed)
                                                  .ToListAsync();                                                  
            return ServiceResponseDto<IEnumerable<Coupon>>.Success(result);
        }

        public async Task<ServiceResponseDto<IEnumerable<Coupon>>> GetActiveCouponsNotUsedByUser(Guid userId)
        {
            var couponUsedByUser = await _uOW.CouponUserRepository.GetAsQueryableIncludedCouponUsed()
                                                                  .AsNoTracking()
                                                                  .Where(cU => cU.UserId == userId)
                                                                  .Select(cU => cU.CouponUsed)
                                                                  .ToListAsync();
            var resultTEmpQuery =  _uOW.CouponRepository.GetAsQueryable()
                                              .AsNoTracking()
                                              .Where(c => c.CouponStatus == Status.Active); //limited the amount of coupon retrived
                                              //TODO find if there is any other way https://stackoverflow.com/a/14682518

            if (couponUsedByUser == null || couponUsedByUser.Count <= 0) {
                return ServiceResponseDto<IEnumerable<Coupon>>.Success(await resultTEmpQuery.ToListAsync());
            }
            var result = await resultTEmpQuery.Except(couponUsedByUser)
                                              .ToListAsync(); 
            //i have to use this to not have error, query cannot be translated: https://learn.microsoft.com/en-us/ef/core/querying/client-eval
            return ServiceResponseDto<IEnumerable<Coupon>>.Success(result);
        }

        public async Task<ServiceResponseDto<Coupon>> MarkUserUsedCoupon(Guid userId, Guid couponId)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            CouponUser couponUser = new CouponUser() { CouponId = couponId, UserId = userId };
            var result = await _uOW.CouponUserRepository.AddAsync(couponUser);
            if (result == null) {
                await transaction.RollbackAsync();
                return ServiceResponseDto<Coupon>.Failure("cannot add coupon user");
            }
            await transaction.CommitAsync();
            var couponUserTemp = await _uOW.CouponUserRepository.GetAsync(couponId: couponId, userId: userId); 
            //this have couponUsed since we used include in the repo.Get()
            Coupon couponUsed = couponUserTemp.CouponUsed;
            return ServiceResponseDto<Coupon>.Success(couponUsed);
        }

        public async Task<ServiceResponseDto<Coupon>> RetrieveValidCouponWithCode(string couponCode)
        {
            IEnumerable<Coupon> resultTemp = await _uOW.CouponRepository.GetAsQueryable()
                                                  .AsNoTracking()
                                                  .Where(c => c.CouponCode == couponCode)
                                                  .Where(c => c.CouponStatus == Status.Active)
                                                  //.Where(c => c.DateEnded >= DateTime.Now) please inspect why this can have errors
                                                  .Select(c => c)
                                                  .ToListAsync();
            if (resultTemp == null ) {
                return ServiceResponseDto<Coupon>.Failure("no coupon match");
            }
            if (resultTemp.Count() > 1) {
                return ServiceResponseDto<Coupon>.Failure("multiple coupon with the same code");
            }
            var result = resultTemp.FirstOrDefault();
            return ServiceResponseDto<Coupon>.Success(result);
        }
    }
}
