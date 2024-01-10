using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public interface ILikeRepository
    {
        IQueryable<Like> GetAllAsQueryable();

        //Task<IEnumerable<Like>> GetLikedOfUserAsync(Guid userId);

        Task<Like> GetAsync(Guid userId, Guid productBusinessKey);

        Task<Like> GetAsync(Guid likeId);

        Task<Like> AddAsync(Guid userId, Guid productBusinessKey, LikeStatus status);

        Task<Like> RemoveAsync(Guid userId, Guid productBusinessKey);

        //specifically , this is update the status
        Task<Like> UpdateAsync(Guid userId, Guid productBusinessKey, LikeStatus updatedLikeStatus);
    }
}
