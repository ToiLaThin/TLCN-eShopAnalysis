using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public interface ICommentService
    {
        Task<ServiceResponseDto<Comment>> Get(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Comment>> Get(Guid bookmarkId);

        Task<ServiceResponseDto<Comment>> Add(Guid userId, Guid productBusinessKey, string commentDetail);

        Task<ServiceResponseDto<Comment>> Update(Guid userId, Guid productBusinessKey, string updatedCommentDetail);

        Task<ServiceResponseDto<Comment>> Remove(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<IEnumerable<Comment>>> GetCommentsOfUserAsync(Guid userId);

        Task<ServiceResponseDto<IEnumerable<Comment>>> GetCommentsAboutProductAsync(Guid productBusinessKey);
    }
}
