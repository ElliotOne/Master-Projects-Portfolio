using Microsoft.AspNetCore.Mvc;
using Moq;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.MatchingManagement.Controllers;
using StartupTeam.Module.MatchingManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Tests.Shared;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.MatchingManagement.Controllers
{
    public class MatchesControllerTests : TestBase
    {
        private readonly Mock<IMatchService> _matchServiceMock;
        private readonly MatchesController _controller;

        public MatchesControllerTests()
        {
            _matchServiceMock = new Mock<IMatchService>();
            _controller = new MatchesController(_matchServiceMock.Object);
        }

        [Fact]
        public async Task GetTopMatchedApplicants_ShouldReturnOkWithData()
        {
            // Arrange
            var jobAdId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true); // Mock a valid user

            _matchServiceMock.Setup(service => service.GetTopMatchedApplicantsForJob(jobAdId, 10))
                .ReturnsAsync(new List<JobApplicationDto> { new JobApplicationDto() });

            // Act
            var result = await _controller.GetTopMatchedApplicants(jobAdId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTopMatchedApplicants_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            var jobAdId = Guid.NewGuid();
            MockUser(_controller, null, false); // No user ID

            // Act
            var result = await _controller.GetTopMatchedApplicants(jobAdId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTopMatchedApplicants_ShouldReturnNotFound_WhenNoMatchedApplicantsFound()
        {
            // Arrange
            var jobAdId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _matchServiceMock.Setup(service => service.GetTopMatchedApplicantsForJob(jobAdId, 10))
                .ReturnsAsync(new List<JobApplicationDto>());

            // Act
            var result = await _controller.GetTopMatchedApplicants(jobAdId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("No matched applicants found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTopMatchedJobAdsForIndividual_ShouldReturnOkWithData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _matchServiceMock.Setup(service => service.GetTopMatchedJobAdsForIndividual(userId, 10))
                .ReturnsAsync(new List<JobAdvertisementDto> { new JobAdvertisementDto() });

            // Act
            var result = await _controller.GetTopMatchedJobAdsForIndividual();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTopMatchedJobAdsForIndividual_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false); // No user ID

            // Act
            var result = await _controller.GetTopMatchedJobAdsForIndividual();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTopMatchedJobAdsForIndividual_ShouldReturnNotFound_WhenNoMatchedJobAdsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _matchServiceMock.Setup(service => service.GetTopMatchedJobAdsForIndividual(userId, 10))
                .ReturnsAsync(new List<JobAdvertisementDto>());

            // Act
            var result = await _controller.GetTopMatchedJobAdsForIndividual();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("No matched job advertisements found.", apiResponse.Message);
        }
    }
}
