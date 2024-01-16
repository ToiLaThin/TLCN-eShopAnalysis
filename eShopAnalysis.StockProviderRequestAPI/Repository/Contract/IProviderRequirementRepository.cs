using eShopAnalysis.StockProviderRequestAPI.Models;

namespace eShopAnalysis.StockProviderRequestAPI.Repository
{
    public interface IProviderRequirementRepository
    {

        IQueryable<ProviderRequirement> GetAllAsQueryable();

        Task<ProviderRequirement> GetAsync(Guid providerReqId);

        Task<ProviderRequirement> GetByNameAsync(string providerName);

        Task<ProviderRequirement> AddAsync (ProviderRequirement providerReqToAdd);

        Task AddRangeAsync(IEnumerable<ProviderRequirement> providerRequirements);

        Task<bool> DeleteAllAsync();
    }
}
