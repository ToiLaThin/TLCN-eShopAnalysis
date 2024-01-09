using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public interface IBookmarkRepository
    {
        //lazy , not call to db until ToList()
        IQueryable<Bookmark> GetAllAsQueryableAsync();

        Task<IEnumerable<Bookmark>> GetBookmarksOfUserAsync(Guid userId);

        Task<Bookmark> GetAsync(Guid userId, Guid productBusinessKey);

        Task<Bookmark> GetAsync(Guid bookmarkId);

        Task<Bookmark> AddAsync(Guid userId, Guid productBusinessKey);

        Task<Bookmark> RemoveAsync(Guid userId, Guid productBusinessKey);
    }
}

