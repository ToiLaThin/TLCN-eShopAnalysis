using eShopAnalysis.CouponSaleItemAPI.Dto;
using eShopAnalysis.CouponSaleItemAPI.Models;
using eShopAnalysis.CouponSaleItemAPI.Service.BackchannelService;
using eShopAnalysis.CouponSaleItemAPI.UnitOfWork;

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
            var result = _uOW.CouponRepository.Add(coupon);
            if (result == null)
            {
                await transaction.RollbackAsync();
                return ServiceResponseDto<Coupon>.Failure("cannot add coupon, rolled back the transaction");
            }
            else
            {
                await transaction.CommitAsync(); //because return no value
                return ServiceResponseDto<Coupon>.Success(result);
            }
        }

        public async Task<ServiceResponseDto<Coupon>> Delete(Guid coupon)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            var result = _uOW.CouponRepository.Delete(coupon);
            if (result == null)
            {
                await transaction.RollbackAsync();
                return ServiceResponseDto<Coupon>.Failure("cannot delete coupon, rolled back the transaction");
            }
            else
            {
                await transaction.CommitAsync();
                return ServiceResponseDto<Coupon>.Success(result);
            }
        }

        public ServiceResponseDto<IEnumerable<Coupon>> GetAll()
        {
            var result = _uOW.CouponRepository.GetAll();
            return ServiceResponseDto<IEnumerable<Coupon>>.Success(result);
        }

        public ServiceResponseDto<Coupon> GetCoupon(Guid couponId)
        {
            var result = _uOW.CouponRepository.Get(couponId);
            return ServiceResponseDto<Coupon>.Success(result);
        }

        public ServiceResponseDto<IEnumerable<Coupon>> GetCouponUsedByUser(Guid userId)
        {
            var result = _uOW.CouponUserRepository.GetAll()
                                                  .AsQueryable()
                                                  .Where(x => x.UserId == userId)
                                                  .Select(c => c.CouponUsed)
                                                  .ToList();                                                  
            return ServiceResponseDto<IEnumerable<Coupon>>.Success(result);
        }

        public ServiceResponseDto<IEnumerable<Coupon>> GetActiveCouponsNotUsedByUser(Guid userId)
        {
            var couponUsedByUser = _uOW.CouponUserRepository.GetAll().Where(cU => cU.UserId == userId).Select(cU => cU.CouponUsed);
            var result = _uOW.CouponRepository.GetAll()
                                              .AsQueryable()
                                              .Where(c => c.CouponStatus == Status.Active) //limited the amount of coupon retrived
                                              .Except(couponUsedByUser) //TODO find if there is any other way https://stackoverflow.com/a/14682518
                                              .ToList();
            return ServiceResponseDto<IEnumerable<Coupon>>.Success(result);
        }

        public async Task<ServiceResponseDto<Coupon>> MarkUserUsedCoupon(Guid userId, Guid couponId)
        {
            var transaction = await _uOW.BeginTransactionAsync();
            CouponUser couponUser = new CouponUser() { CouponId = couponId, UserId = userId };
            var result = _uOW.CouponUserRepository.Add(couponUser);
            if (result == null)
            {
                await transaction.RollbackAsync();
                return ServiceResponseDto<Coupon>.Failure("cannot add coupon user");
            }
            else
            {
                await transaction.CommitAsync();
                Coupon couponUsed = _uOW.CouponUserRepository.Get(couponId: couponId, userId: userId).CouponUsed; //this have couponUsed since we used include in the repo.Get()
                return ServiceResponseDto<Coupon>.Success(couponUsed);
            }
        }

        public ServiceResponseDto<Coupon> RetrieveValidCouponWithCode(string couponCode)
        {
            IEnumerable<Coupon> resultTemp = _uOW.CouponRepository.GetAll()
                                                  .Where(c => c.CouponCode == couponCode)
                                                  .Where(c => c.CouponStatus == Status.Active)
                                                  .Where(c => c.DateEnded >= DateTime.Now)
                                                  .Select(c => c);
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
