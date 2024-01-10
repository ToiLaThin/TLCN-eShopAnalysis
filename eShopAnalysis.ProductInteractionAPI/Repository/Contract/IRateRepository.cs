using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Repository
{
    public interface IRateRepository
    {
        //lazy , not call to db until ToList()
        //the logic of did the user buy the product or not before they can rate
        //that is on frontend and on aggregate service
        IQueryable<Rate> GetAllAsQueryableAsync();

        //TODO the repo may have only the basic of operation, these kind of logic should be moved into the service layer
        //Task<IEnumerable<Rate>> GetRatedMappingsOfUserAsync(Guid userId);
        //Task<IEnumerable<Rate>> GetRatedMappingsOfProductAsync(Guid productBusinessKey);

        Task<Rate> GetAsync(Guid userId, Guid productBusinessKey);

        Task<Rate> GetAsync(Guid rateId);

        Task<Rate> AddAsync(Guid userId, Guid productBusinessKey, double rating);

        Task<Rate> RemoveAsync(Guid userId, Guid productBusinessKey);

        //updated ? maybe later
    }
}
