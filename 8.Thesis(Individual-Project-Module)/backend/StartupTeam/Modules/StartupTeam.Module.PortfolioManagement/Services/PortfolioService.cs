using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.PortfolioManagement.Data;
using StartupTeam.Module.PortfolioManagement.Dtos;
using StartupTeam.Module.PortfolioManagement.Models;
using StartupTeam.Shared.Helpers;
using StartupTeam.Shared.Models.Constants;
using StartupTeam.Shared.Services;

namespace StartupTeam.Module.PortfolioManagement.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly PortfolioManagementDbContext _context;
        private readonly IBlobStorageService _blobStorageService;

        public PortfolioService(
            PortfolioManagementDbContext context,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public async Task<IEnumerable<PortfolioItemDto>> GetPortfolioItemsByUserIdAsync(Guid userId)
        {
            return await _context.Portfolios
                .Where(p => p.UserId == userId)
                .Include(p => p.PortfolioItems)
                .SelectMany(p => p.PortfolioItems)
                .Select(item => new PortfolioItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                })
                .ToListAsync();
        }

        public async Task<PortfolioItemFormDto?> GetPortfolioItemFormByIdAsync(Guid id)
        {
            var portfolioItem = await _context.PortfolioItems
                .Include(pi => pi.Portfolio)
                .FirstOrDefaultAsync(pi => pi.Id == id);

            if (portfolioItem == null)
            {
                return null;
            }

            return new PortfolioItemFormDto
            {
                Id = portfolioItem.Id,
                Title = portfolioItem.Title,
                Description = portfolioItem.Description,
                Type = portfolioItem.Type,
                Technologies = portfolioItem.Technologies,
                Skills = portfolioItem.Skills,
                Industry = portfolioItem.Industry,
                Role = portfolioItem.Role,
                Duration = portfolioItem.Duration,
                Link = portfolioItem.Link,
                AttachmentUrl = portfolioItem.AttachmentUrl,
                Tags = portfolioItem.Tags,
                UserId = portfolioItem.Portfolio.UserId
            };
        }

        public async Task<PortfolioItemDetailDto?> GetPortfolioItemByIdAsync(Guid id)
        {
            var portfolioItem = await _context.PortfolioItems.FindAsync(id);

            if (portfolioItem == null)
            {
                return null;
            }

            var portfolioItemDetailDto = new PortfolioItemDetailDto()
            {
                Id = portfolioItem.Id,
                Title = portfolioItem.Title,
                Description = portfolioItem.Description,
                Type = portfolioItem.Type,
                Technologies = portfolioItem.Technologies,
                Skills = portfolioItem.Skills,
                Industry = portfolioItem.Industry,
                Role = portfolioItem.Role,
                Duration = portfolioItem.Duration,
                Link = portfolioItem.Link,
                AttachmentUrl = portfolioItem.AttachmentUrl,
                Tags = portfolioItem.Tags
            };

            return portfolioItemDetailDto;
        }

        public async Task<bool> CreatePortfolioItemAsync(PortfolioItemFormDto portfolioItemFormDto, Guid userId)
        {
            string? attachmentUrl = null;

            if (portfolioItemFormDto.AttachmentFile != null)
            {
                await using var stream = portfolioItemFormDto.AttachmentFile.OpenReadStream();
                attachmentUrl = await _blobStorageService.UploadBlobAsync(
                    BlobStorageConstants.PortfolioAttachmentsContainerName,
                    FormFileHelper.GenerateNewFileName(portfolioItemFormDto.AttachmentFile),
                    stream);
            }

            var existingPortfolio = await _context.Portfolios
                .FirstOrDefaultAsync(p => p.UserId == userId);

            var portfolioItem = new PortfolioItem
            {
                Title = portfolioItemFormDto.Title,
                Description = portfolioItemFormDto.Description,
                Type = portfolioItemFormDto.Type,
                Technologies = portfolioItemFormDto.Technologies,
                Skills = portfolioItemFormDto.Skills,
                Industry = portfolioItemFormDto.Industry,
                Role = portfolioItemFormDto.Role,
                Duration = portfolioItemFormDto.Duration,
                Link = portfolioItemFormDto.Link,
                AttachmentUrl = attachmentUrl,
                Tags = portfolioItemFormDto.Tags,

                PortfolioId = existingPortfolio?.Id ?? default,
                Portfolio = existingPortfolio ?? new Portfolio()
                {
                    UserId = userId
                }
            };

            _context.PortfolioItems.Add(portfolioItem);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePortfolioItemAsync(PortfolioItemFormDto portfolioItemFormDto)
        {
            var portfolioItem = await _context.PortfolioItems.FindAsync(portfolioItemFormDto.Id);

            if (portfolioItem == null)
            {
                return false;
            }

            // Handle attachment update
            if (portfolioItemFormDto.AttachmentFile != null)
            {
                // Delete old attachment if it exists
                if (!string.IsNullOrEmpty(portfolioItemFormDto.AttachmentUrl))
                {
                    var oldBlobName = Path.GetFileName(portfolioItemFormDto.AttachmentUrl);
                    await _blobStorageService.DeleteBlobAsync(BlobStorageConstants.PortfolioAttachmentsContainerName, oldBlobName);
                }

                // Upload new attachment
                await using var stream = portfolioItemFormDto.AttachmentFile.OpenReadStream();
                portfolioItemFormDto.AttachmentUrl = await _blobStorageService.UploadBlobAsync(
                    BlobStorageConstants.PortfolioAttachmentsContainerName,
                    FormFileHelper.GenerateNewFileName(portfolioItemFormDto.AttachmentFile),
                    stream);
            }

            portfolioItem.Title = portfolioItemFormDto.Title;
            portfolioItem.Description = portfolioItemFormDto.Description;
            portfolioItem.Type = portfolioItemFormDto.Type;
            portfolioItem.Technologies = portfolioItemFormDto.Technologies;
            portfolioItem.Skills = portfolioItemFormDto.Skills;
            portfolioItem.Industry = portfolioItemFormDto.Industry;
            portfolioItem.Role = portfolioItemFormDto.Role;
            portfolioItem.Duration = portfolioItemFormDto.Duration;
            portfolioItem.Link = portfolioItemFormDto.Link;
            portfolioItem.AttachmentUrl = portfolioItemFormDto.AttachmentUrl;
            portfolioItem.Tags = portfolioItemFormDto.Tags;

            _context.Entry(portfolioItem).State = EntityState.Modified;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePortfolioItemAsync(Guid id)
        {
            var portfolioItem = await _context.PortfolioItems.FindAsync(id);

            if (portfolioItem == null)
            {
                return false;
            }

            // Delete attachment if it exists
            if (!string.IsNullOrEmpty(portfolioItem.AttachmentUrl))
            {
                var oldBlobName = Path.GetFileName(portfolioItem.AttachmentUrl);
                await _blobStorageService.DeleteBlobAsync(BlobStorageConstants.PortfolioAttachmentsContainerName, oldBlobName);
            }

            _context.PortfolioItems.Remove(portfolioItem);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
