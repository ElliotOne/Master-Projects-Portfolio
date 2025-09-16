using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using StartupTeam.Module.PortfolioManagement.Data;
using StartupTeam.Module.PortfolioManagement.Dtos;
using StartupTeam.Module.PortfolioManagement.Models;
using StartupTeam.Module.PortfolioManagement.Services;
using StartupTeam.Shared.Services;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.PortfolioManagement.Services
{
    public class PortfolioServiceTests
    {
        private PortfolioService _portfolioService;
        private PortfolioManagementDbContext _dbContext;
        private Mock<IBlobStorageService> _blobStorageServiceMock;

        public void InitializeDatabase()
        {
            // Use a new InMemoryDatabase for each test, ensuring unique database name
            var options = new DbContextOptionsBuilder<PortfolioManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB name for each test
                .Options;

            _dbContext = new PortfolioManagementDbContext(options);
            _blobStorageServiceMock = new Mock<IBlobStorageService>();

            _portfolioService = new PortfolioService(_dbContext, _blobStorageServiceMock.Object);
        }

        private void SeedData()
        {
            var userId = Guid.NewGuid();
            _dbContext.Portfolios.Add(new Portfolio
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PortfolioItems = new List<PortfolioItem>
                {
                    new PortfolioItem
                    {
                        Id = Guid.NewGuid(),
                        Title = "Portfolio Item 1",
                        Description = "Description 1",
                        Skills = "C#, .NET",
                        Industry = "Tech",
                    }
                }
            });

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetPortfolioItemsByUserIdAsync_ShouldReturnItems_WhenItemsExist()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var userId = _dbContext.Portfolios.First().UserId;

            // Act
            var result = await _portfolioService.GetPortfolioItemsByUserIdAsync(userId);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal("Portfolio Item 1", result.First().Title);
        }

        [Fact]
        public async Task GetPortfolioItemFormByIdAsync_ShouldReturnFormDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var itemId = _dbContext.PortfolioItems.First().Id;

            // Act
            var result = await _portfolioService.GetPortfolioItemFormByIdAsync(itemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Portfolio Item 1", result.Title);
        }

        [Fact]
        public async Task GetPortfolioItemByIdAsync_ShouldReturnDetailDto_WhenFound()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var itemId = _dbContext.PortfolioItems.First().Id;

            // Act
            var result = await _portfolioService.GetPortfolioItemByIdAsync(itemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Portfolio Item 1", result.Title);
        }

        [Fact]
        public async Task CreatePortfolioItemAsync_ShouldReturnTrue_WhenItemCreatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();

            var userId = Guid.NewGuid();
            var formDto = new PortfolioItemFormDto
            {
                Title = "New Portfolio Item",
                Description = "New Description",
                Skills = "React, JavaScript",
            };

            // Act
            var result = await _portfolioService.CreatePortfolioItemAsync(formDto, userId);

            // Assert
            Assert.True(result);
            Assert.Equal(1, _dbContext.PortfolioItems.Count());
        }

        [Fact]
        public async Task CreatePortfolioItemAsync_ShouldUploadBlob_WhenAttachmentProvided()
        {
            // Arrange
            InitializeDatabase();

            var userId = Guid.NewGuid();
            var formDto = new PortfolioItemFormDto
            {
                Title = "New Portfolio Item",
                AttachmentFile = new Mock<IFormFile>().Object
            };

            _blobStorageServiceMock
                .Setup(service => service.UploadBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync("mock-url");

            // Act
            var result = await _portfolioService.CreatePortfolioItemAsync(formDto, userId);

            // Assert
            Assert.True(result);
            Assert.NotNull(_dbContext.PortfolioItems.First().AttachmentUrl);
        }

        [Fact]
        public async Task UpdatePortfolioItemAsync_ShouldReturnTrue_WhenItemUpdatedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var itemId = _dbContext.PortfolioItems.First().Id;
            var formDto = new PortfolioItemFormDto
            {
                Id = itemId,
                Title = "Updated Portfolio Item",
                Description = "Updated Description"
            };

            // Act
            var result = await _portfolioService.UpdatePortfolioItemAsync(formDto);

            // Assert
            Assert.True(result);
            var updatedItem = await _dbContext.PortfolioItems.FindAsync(itemId);
            Assert.Equal("Updated Portfolio Item", updatedItem.Title);
        }

        [Fact]
        public async Task UpdatePortfolioItemAsync_ShouldReturnFalse_WhenItemNotFound()
        {
            // Arrange
            InitializeDatabase();

            var nonExistentItemId = Guid.NewGuid(); // This ID doesn't exist
            var formDto = new PortfolioItemFormDto
            {
                Id = nonExistentItemId,
                Title = "Updated Portfolio Item"
            };

            // Act
            var result = await _portfolioService.UpdatePortfolioItemAsync(formDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeletePortfolioItemAsync_ShouldReturnTrue_WhenItemDeletedSuccessfully()
        {
            // Arrange
            InitializeDatabase();
            SeedData();

            var itemId = _dbContext.PortfolioItems.First().Id;

            // Act
            var result = await _portfolioService.DeletePortfolioItemAsync(itemId);

            // Assert
            Assert.True(result);
            Assert.Null(await _dbContext.PortfolioItems.FindAsync(itemId));
        }

        [Fact]
        public async Task DeletePortfolioItemAsync_ShouldReturnFalse_WhenItemNotFound()
        {
            // Arrange
            InitializeDatabase();

            var nonExistentItemId = Guid.NewGuid(); // This ID doesn't exist

            // Act
            var result = await _portfolioService.DeletePortfolioItemAsync(nonExistentItemId);

            // Assert
            Assert.False(result);
        }
    }
}
