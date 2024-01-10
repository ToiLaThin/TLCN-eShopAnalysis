using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;
using eShopAnalysis.ProductInteractionAPI.Repository;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public class LikeService : ILikeService
    {

        private readonly ILikeRepository _likedMappingRepository;
        public LikeService(ILikeRepository likedMappingRepository)
        {
            _likedMappingRepository = likedMappingRepository;
        }
        public async Task<ServiceResponseDto<Like>> Add(Guid userId, Guid productBusinessKey)
        {
            bool isLikedMappingExisted = await _likedMappingRepository.GetAsync(userId, productBusinessKey) != null;
            if (isLikedMappingExisted == true) {
                return ServiceResponseDto<Like>.Failure("Cannot added likedMapping because user already liked this product");
            }
            var likedMappingAdded = await _likedMappingRepository.AddAsync(userId, productBusinessKey);
            if (likedMappingAdded == null) {
                return ServiceResponseDto<Like>.Failure("Cannot added likedMapping because cannot find it");
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
            var likedMappingsOfUser = await _likedMappingRepository.GetLikedOfUserAsync(userId);
            if (likedMappingsOfUser == null) {
                return ServiceResponseDto<IEnumerable<Like>>.Failure("likedMapping list of user is null");
            }
            return ServiceResponseDto<IEnumerable<Like>>.Success(likedMappingsOfUser);
        }

        public async Task<ServiceResponseDto<Like>> Remove(Guid userId, Guid productBusinessKey)
        {
            var likedMappingDeleted = await _likedMappingRepository.RemoveAsync(userId, productBusinessKey);
            if (likedMappingDeleted == null) {
                return ServiceResponseDto<Like>.Failure("cannot delete likedMapping, please check result of repo");
            }
            return ServiceResponseDto<Like>.Success(likedMappingDeleted);
        }
    }
}
