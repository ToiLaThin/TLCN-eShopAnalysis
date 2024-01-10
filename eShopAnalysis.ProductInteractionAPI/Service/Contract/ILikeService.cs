using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public interface ILikeService
    {
        Task<ServiceResponseDto<Like>> Get(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Like>> Get(Guid likeId);

        Task<ServiceResponseDto<Like>> Add(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Like>> Remove(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<IEnumerable<Like>>> GetLikedMappingsOfUserAsync(Guid userId);
    }
}
