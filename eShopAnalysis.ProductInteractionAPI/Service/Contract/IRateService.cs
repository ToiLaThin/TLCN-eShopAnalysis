using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public interface IRateService
    {
        Task<ServiceResponseDto<Rate>> Get(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Rate>> Get(Guid rateId);

        /// <summary>
        /// This is more like a upsert operation, if exist then we update.Otherwise, add a new instance
        /// </summary>
        /// <param name="userId">userId of the user who rate the product</param>
        /// <param name="productBusinessKey">productBusinessKey is the key to represent product (not productId because this change when product is updated)</param>
        /// <param name="rating">the valid double type rating of this: 0 -> 5 </param>
        /// <returns></returns>
        Task<ServiceResponseDto<Rate>> RateProductFromUser(Guid userId, Guid productBusinessKey, double rating);

        Task<ServiceResponseDto<Rate>> Remove(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<IEnumerable<Rate>>> GetRatedMappingsOfUserAsync(Guid userId);

        Task<ServiceResponseDto<IEnumerable<Rate>>> GetRatedMappingsAboutProductAsync(Guid productBusinessKey);
    }
}
