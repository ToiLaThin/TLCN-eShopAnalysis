using eShopAnalysis.ProductInteractionAPI.Dto;
using eShopAnalysis.ProductInteractionAPI.Models;

namespace eShopAnalysis.ProductInteractionAPI.Service
{
    public interface IRateService
    {
        Task<ServiceResponseDto<Rate>> Get(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<Rate>> Get(Guid rateId);

        Task<ServiceResponseDto<Rate>> Add(Guid userId, Guid productBusinessKey, double rating);

        Task<ServiceResponseDto<Rate>> Remove(Guid userId, Guid productBusinessKey);

        Task<ServiceResponseDto<IEnumerable<Rate>>> GetRatedMappingsOfUserAsync(Guid userId);

        Task<ServiceResponseDto<IEnumerable<Rate>>> GetRatedMappingsAboutProductAsync(Guid productBusinessKey);
    }
}
