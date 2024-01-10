using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public interface ILikeRepository
    {
        //lazy , not call to db until ToList()
        //SORRY THIS LINE IS TRUE IN EF Core but I do not know for sure in MongoDb Driver for .NET
        IQueryable<Like> GetAllAsQueryableAsync();

        Task<IEnumerable<Like>> GetLikedOfUserAsync(Guid userId);

        Task<Like> GetAsync(Guid userId, Guid productBusinessKey);

        Task<Like> GetAsync(Guid likeId);

        Task<Like> AddAsync(Guid userId, Guid productBusinessKey);

        Task<Like> RemoveAsync(Guid userId, Guid productBusinessKey);
    }
}
