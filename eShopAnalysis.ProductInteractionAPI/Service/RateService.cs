using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Repository;
using System.ComponentModel.Design;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public class RateService : IRateService
    {
        private readonly IRateRepository _rateRepository;

        public RateService(IRateRepository rateRepository)
        {
            _rateRepository = rateRepository;
        }
        public async Task<ServiceResponseDto<Rate>> RateProductFromUser(Guid userId, Guid productBusinessKey, double rating)
        {
            //we can add logic that check the validity of rating here
            if (rating < 0 || rating > 5) {
                return ServiceResponseDto<Rate>.Failure("Cannot added rate because rating is not in valid range");
            }

            Rate? rateToFind = await _rateRepository.GetAsync(userId, productBusinessKey);
            bool rateExisted = rateToFind != null;
            if (rateExisted == true) {
                //update NOT add instance since user already rate the product
                Rate? rateUpdated = await _rateRepository.UpdateAsync(rateToFind, rating);
                if (rateUpdated == null) {
                    return ServiceResponseDto<Rate>.Failure("Cannot update existing rate because cannot find it or something, please check the repo");
                }
                return ServiceResponseDto<Rate>.Success(rateUpdated);
            }

            // add if no instance mapping between user and product
            var rateAdded = await _rateRepository.AddAsync(userId, productBusinessKey, rating);
            if (rateAdded == null) {
                return ServiceResponseDto<Rate>.Failure("Cannot added rate because cannot find it");
            }
            return ServiceResponseDto<Rate>.Success(rateAdded);
        }

        public async Task<ServiceResponseDto<Rate>> Get(Guid userId, Guid productBusinessKey)
        {
            Rate rateToFind = await _rateRepository.GetAsync(userId, productBusinessKey);
            if (rateToFind == null) {
                return ServiceResponseDto<Rate>.Failure("Cannot find user rate about product");
            }
            return ServiceResponseDto<Rate>.Success(rateToFind);
        }

        public async Task<ServiceResponseDto<Rate>> Get(Guid rateId)
        {
            Rate rateToFind = await _rateRepository.GetAsync(rateId);
            if (rateToFind == null) {
                return ServiceResponseDto<Rate>.Failure("Cannot find rate of that id");
            }
            return ServiceResponseDto<Rate>.Success(rateToFind);
        }

        public async Task<ServiceResponseDto<IEnumerable<Rate>>> GetRatedMappingsAboutProductAsync(Guid productBusinessKey)
        {
            var ratesAboutProduct = _rateRepository.GetAllAsQueryable()
                                                   .Where(c => c.ProductBusinessKey.Equals(productBusinessKey))
                                                   .ToList();
            if (ratesAboutProduct == null) {
                return ServiceResponseDto<IEnumerable<Rate>>.Failure("The rate list about product is null, not even empty");
            }
            return ServiceResponseDto<IEnumerable<Rate>>.Success(ratesAboutProduct);
        }

        public async Task<ServiceResponseDto<IEnumerable<Rate>>> GetRatedMappingsOfUserAsync(Guid userId)
        {
            var ratesOfUser = _rateRepository.GetAllAsQueryable()
                                             .Where(c => c.UserId.Equals(userId))
                                             .ToList();
            if (ratesOfUser == null) {
                return ServiceResponseDto<IEnumerable<Rate>>.Failure("The user 's rate list is null, not even empty");
            }
            return ServiceResponseDto<IEnumerable<Rate>>.Success(ratesOfUser);
        }

        public async Task<ServiceResponseDto<Rate>> Remove(Guid userId, Guid productBusinessKey)
        {
            var rateDeleted = await _rateRepository.RemoveAsync(userId, productBusinessKey);
            if (rateDeleted == null) {
                return ServiceResponseDto<Rate>.Failure("cannot delete rate, please check result of repo");
            }
            return ServiceResponseDto<Rate>.Success(rateDeleted);
        }
    }
}
