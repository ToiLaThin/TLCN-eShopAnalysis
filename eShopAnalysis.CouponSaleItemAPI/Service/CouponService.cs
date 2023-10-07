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


    }
}
