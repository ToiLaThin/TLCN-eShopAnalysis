using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Repository;
using MongoDB.Driver.Linq;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public class LikeService : ILikeService
    {

        private readonly ILikeRepository _likedMappingRepository;
        public LikeService(ILikeRepository likedMappingRepository)
        {
            _likedMappingRepository = likedMappingRepository;
        }

        public async Task<ServiceResponseDto<Like>> DisLikeProductFromUser(Guid userId, Guid productBusinessKey)
        {
            bool isLikedMappingExisted = await _likedMappingRepository.GetAsync(userId, productBusinessKey) != null;
            if (isLikedMappingExisted == true) {
                //update to dislike if already exist
                var updatedResult = await _likedMappingRepository.UpdateAsync(userId, productBusinessKey, LikeStatus.Disliked);
                if (updatedResult == null) {
                    return ServiceResponseDto<Like>.Failure("cannot updated because cannot find the . Please check the repo result");
                }
                return ServiceResponseDto<Like>.Success(updatedResult);
            }

            //if not exist create with init status is dislike
            var likedMappingAdded = await _likedMappingRepository.AddAsync(userId, productBusinessKey, LikeStatus.Disliked);
            if (likedMappingAdded == null) {
                return ServiceResponseDto<Like>.Failure("Cannot added likedMapping status dislike because cannot find it");
            }
            return ServiceResponseDto<Like>.Success(likedMappingAdded);
        }

        public async Task<ServiceResponseDto<Like>> Get(Guid userId, Guid productBusinessKey)
        {
            Like likedMappingToFind = await _likedMappingRepository.GetAsync(userId, productBusinessKey);
            if (likedMappingToFind == null) {
                return ServiceResponseDto<Like>.Failure("Cannot find likedMapping of user and product");
            }
            return ServiceResponseDto<Like>.Success(likedMappingToFind);
        }

        public async Task<ServiceResponseDto<Like>> Get(Guid likeId)
        {
            Like likedMappingToFind = await _likedMappingRepository.GetAsync(likeId);
            if (likedMappingToFind == null) {
                return ServiceResponseDto<Like>.Failure("Cannot find likedMapping of that id");
            }
            return ServiceResponseDto<Like>.Success(likedMappingToFind);
        }

        public async Task<ServiceResponseDto<IEnumerable<Like>>> GetLikedMappingsOfUserAsync(Guid userId)
        {
            //even though we do not have await, we can still have async. The reverse is not true
            var likedMappingsOfUser = _likedMappingRepository.GetAllAsQueryable()
                                                             .Where(l => l.UserId.Equals(userId))
                                                             .ToList();
            if (likedMappingsOfUser == null) {
                return ServiceResponseDto<IEnumerable<Like>>.Failure("likedMapping list of user is null");
            }
            return ServiceResponseDto<IEnumerable<Like>>.Success(likedMappingsOfUser);
        }

        public async Task<ServiceResponseDto<Like>> LikeProductFromUser(Guid userId, Guid productBusinessKey)
        {
            bool isLikedMappingExisted = await _likedMappingRepository.GetAsync(userId, productBusinessKey) != null;
            if (isLikedMappingExisted == true) {
                //update to like if already exist
                var updatedResult = await _likedMappingRepository.UpdateAsync(userId, productBusinessKey, LikeStatus.Liked);
                if (updatedResult == null) {
                    return ServiceResponseDto<Like>.Failure("cannot updated because cannot find the . Please check the repo result");
                }
                return ServiceResponseDto<Like>.Success(updatedResult);
            }

            //if not exist create with init status is like
            var likedMappingAdded = await _likedMappingRepository.AddAsync(userId, productBusinessKey, LikeStatus.Liked);
            if (likedMappingAdded == null) {
                return ServiceResponseDto<Like>.Failure("Cannot added likedMapping status like because cannot find it");
            }
            return ServiceResponseDto<Like>.Success(likedMappingAdded);
        }

        public async Task<ServiceResponseDto<Like>> Remove(Guid userId, Guid productBusinessKey)
        {
            var likedMappingDeleted = await _likedMappingRepository.RemoveAsync(userId, productBusinessKey);
            if (likedMappingDeleted == null) {
                return ServiceResponseDto<Like>.Failure("cannot delete likedMapping, please check result of repo");
            }
            return ServiceResponseDto<Like>.Success(likedMappingDeleted);
        }

        public async Task<ServiceResponseDto<Like>> UnLikeProductFromUser(Guid userId, Guid productBusinessKey)
        {
            bool isLikedMappingExisted = await _likedMappingRepository.GetAsync(userId, productBusinessKey) != null;
            if (isLikedMappingExisted == true) {
                //update to neutral if already exist
                var updatedResult = await _likedMappingRepository.UpdateAsync(userId, productBusinessKey, LikeStatus.Neutral);
                if (updatedResult == null) {
                    return ServiceResponseDto<Like>.Failure("cannot updated because cannot find the . Please check the repo result");
                }
                return ServiceResponseDto<Like>.Success(updatedResult);
            }

            //if not exist create with init status is neutral
            var likedMappingAdded = await _likedMappingRepository.AddAsync(userId, productBusinessKey, LikeStatus.Neutral);
            if (likedMappingAdded == null) { 
                return ServiceResponseDto<Like>.Failure("Cannot added likedMapping status neutral because cannot find it");
            }
            return ServiceResponseDto<Like>.Success(likedMappingAdded);
        }
    }
}
