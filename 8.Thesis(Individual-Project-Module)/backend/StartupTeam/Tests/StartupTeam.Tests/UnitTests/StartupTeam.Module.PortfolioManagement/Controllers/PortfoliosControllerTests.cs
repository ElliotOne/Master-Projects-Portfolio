using Microsoft.AspNetCore.Mvc;
using Moq;
using StartupTeam.Module.PortfolioManagement.Controllers;
using StartupTeam.Module.PortfolioManagement.Dtos;
using StartupTeam.Module.PortfolioManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Tests.Shared;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.PortfolioManagement.Controllers
{
    public class PortfoliosControllerTests : TestBase
    {
        private readonly Mock<IPortfolioService> _portfolioServiceMock;
        private readonly PortfoliosController _controller;

        public PortfoliosControllerTests()
        {
            _portfolioServiceMock = new Mock<IPortfolioService>();
            _controller = new PortfoliosController(_portfolioServiceMock.Object);
        }

        [Fact]
        public async Task GetPortfolioItems_ShouldReturnOkWithData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _portfolioServiceMock.Setup(service => service.GetPortfolioItemsByUserIdAsync(userId))
                .ReturnsAsync(new List<PortfolioItemDto> { new PortfolioItemDto() });

            // Act
            var result = await _controller.GetPortfolioItems();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetPortfolioItems_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);  // No user

            // Act
            var result = await _controller.GetPortfolioItems();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetPortfolioItemsByUserId_ShouldReturnOkWithData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.GetPortfolioItemsByUserIdAsync(userId))
                .ReturnsAsync(new List<PortfolioItemDto> { new PortfolioItemDto() });

            // Act
            var result = await _controller.GetPortfolioItemsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetPortfolioItemsByUserId_ShouldReturnNotFound_WhenNoPortfolioItemsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.GetPortfolioItemsByUserIdAsync(userId))
                .ReturnsAsync(new List<PortfolioItemDto>());

            // Act
            var result = await _controller.GetPortfolioItemsByUserId(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("No portfolio items found for this user.", apiResponse.Message);
        }

        [Fact]
        public async Task GetPortfolioItemForm_ShouldReturnNotFound_WhenPortfolioItemNotFound()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.GetPortfolioItemFormByIdAsync(portfolioItemId))
                .ReturnsAsync((PortfolioItemFormDto?)null); // Portfolio item not found

            // Act
            var result = await _controller.GetPortfolioItemForm(portfolioItemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Portfolio item not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetPortfolioItemForm_ShouldReturnOkWithData_WhenPortfolioItemIsFound()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.GetPortfolioItemFormByIdAsync(portfolioItemId))
                .ReturnsAsync(new PortfolioItemFormDto { Id = portfolioItemId });

            // Act
            var result = await _controller.GetPortfolioItemForm(portfolioItemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PortfolioItemFormDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(portfolioItemId, apiResponse.Data.Id);
        }

        [Fact]
        public async Task CreatePortfolioItem_ShouldReturnOk_WhenCreationIsSuccessful()
        {
            // Arrange
            var portfolioItemFormDto = new PortfolioItemFormDto { Id = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _portfolioServiceMock.Setup(service => service.CreatePortfolioItemAsync(portfolioItemFormDto, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreatePortfolioItem(portfolioItemFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Portfolio item created successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task CreatePortfolioItem_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.CreatePortfolioItem(new PortfolioItemFormDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task CreatePortfolioItem_ShouldReturnServerError_WhenCreationFails()
        {
            // Arrange
            var portfolioItemFormDto = new PortfolioItemFormDto { Id = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _portfolioServiceMock.Setup(service => service.CreatePortfolioItemAsync(portfolioItemFormDto, userId))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.CreatePortfolioItem(portfolioItemFormDto);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<object>>(serverErrorResult.Value);
            Assert.Equal("An error occurred while creating the portfolio item.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdatePortfolioItem_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var portfolioItemFormDto = new PortfolioItemFormDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var userId = portfolioItemFormDto.UserId;
            MockUser(_controller, userId, true);

            _portfolioServiceMock.Setup(service => service.UpdatePortfolioItemAsync(portfolioItemFormDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePortfolioItem(portfolioItemFormDto.Id.Value, portfolioItemFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Portfolio item updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdatePortfolioItem_ShouldReturnUnauthorized_WhenUserIdIsMismatch()
        {
            // Arrange
            var portfolioItemFormDto = new PortfolioItemFormDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            // Act
            var result = await _controller.UpdatePortfolioItem(portfolioItemFormDto.Id.Value, portfolioItemFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to update this portfolio item.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdatePortfolioItem_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            var portfolioItemFormDto = new PortfolioItemFormDto { Id = Guid.NewGuid() }; // Different ID

            MockUser(_controller, Guid.NewGuid(), true);

            // Act
            var result = await _controller.UpdatePortfolioItem(portfolioItemId, portfolioItemFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("ID mismatch.", apiResponse.Message);
        }

        [Fact]
        public async Task DeletePortfolioItem_ShouldReturnOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.DeletePortfolioItemAsync(portfolioItemId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePortfolioItem(portfolioItemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Portfolio item deleted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task DeletePortfolioItem_ShouldReturnNotFound_WhenPortfolioItemNotFound()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.DeletePortfolioItemAsync(portfolioItemId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePortfolioItem(portfolioItemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Portfolio item not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetPortfolioItemDetail_ShouldReturnNotFound_WhenPortfolioItemNotFound()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.GetPortfolioItemByIdAsync(portfolioItemId))
                .ReturnsAsync((PortfolioItemDetailDto?)null); // Portfolio item not found

            // Act
            var result = await _controller.GetPortfolioItemDetail(portfolioItemId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Portfolio item not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetPortfolioItemDetail_ShouldReturnOkWithData_WhenPortfolioItemIsFound()
        {
            // Arrange
            var portfolioItemId = Guid.NewGuid();
            _portfolioServiceMock.Setup(service => service.GetPortfolioItemByIdAsync(portfolioItemId))
                .ReturnsAsync(new PortfolioItemDetailDto { Id = portfolioItemId });

            // Act
            var result = await _controller.GetPortfolioItemDetail(portfolioItemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<PortfolioItemDetailDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
            Assert.Equal(portfolioItemId, apiResponse.Data.Id);
        }
    }
}
