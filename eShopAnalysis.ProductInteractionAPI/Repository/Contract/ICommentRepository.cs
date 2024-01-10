using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public interface ICommentRepository
    {
        //TODO test if getAsQueryable called to db by debug and check when the query is printed out
        //but i THINK mostly it is more likely to exec after ToList()
        IQueryable<Comment> GetAllAsQueryableAsync();

        //the repo may have only the basic of operation, these kind of logic should be moved into the service layer
        //Task<IEnumerable<Comment>> GetCommentsOfUserAsync(Guid userId);
        //Task<IEnumerable<Comment>> GetCommentsAboutProductAsync(Guid productBusinessKey);


        //each pair of userId & productBusinessKey is unique
        Task<Comment> GetAsync(Guid userId, Guid productBusinessKey);

        Task<Comment> GetAsync(Guid commentId);

        Task<Comment> AddAsync(Guid userId, Guid productBusinessKey, string commentDetail);

        Task<Comment> RemoveAsync(Guid userId, Guid productBusinessKey);

        Task<Comment> UpdateAsync(Guid userId, Guid productBusinessKey, string updatedCommentDetail);
    }
}
