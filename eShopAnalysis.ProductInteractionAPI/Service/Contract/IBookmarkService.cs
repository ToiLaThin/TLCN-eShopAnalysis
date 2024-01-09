using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public interface IBookmarkService
    {
        Task<ServiceResponseDto<Bookmark>> Get(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Bookmark>> Get(Guid bookmarkId);

        Task<ServiceResponseDto<Bookmark>> Add(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Bookmark>> Remove(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<IEnumerable<Bookmark>>> GetBookmarksOfUserAsync(Guid userId);
    }
}
