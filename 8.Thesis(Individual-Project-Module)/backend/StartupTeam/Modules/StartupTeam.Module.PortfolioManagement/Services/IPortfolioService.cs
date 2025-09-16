using StartupTeam.Module.PortfolioManagement.Dtos;

namespace StartupTeam.Module.PortfolioManagement.Services
{
    public interface IPortfolioService
    {
        Task<IEnumerable<PortfolioItemDto>> GetPortfolioItemsByUserIdAsync(Guid userId);
        Task<PortfolioItemFormDto?> GetPortfolioItemFormByIdAsync(Guid id);
        Task<PortfolioItemDetailDto?> GetPortfolioItemByIdAsync(Guid id);
        Task<bool> CreatePortfolioItemAsync(PortfolioItemFormDto portfolioItemFormDto, Guid userId);
        Task<bool> UpdatePortfolioItemAsync(PortfolioItemFormDto portfolioItemFormDto);
        Task<bool> DeletePortfolioItemAsync(Guid id);
    }
}
