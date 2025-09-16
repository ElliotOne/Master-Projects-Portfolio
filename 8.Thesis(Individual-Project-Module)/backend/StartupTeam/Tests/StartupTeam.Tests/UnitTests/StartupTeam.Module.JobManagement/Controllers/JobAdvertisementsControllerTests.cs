using Microsoft.AspNetCore.Mvc;
using Moq;
using StartupTeam.Module.JobManagement.Controllers;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Tests.Shared;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Controllers
{
    public class JobAdvertisementsControllerTests : TestBase
    {
        private readonly Mock<IJobService> _jobServiceMock;
        private readonly JobAdvertisementsController _controller;

        public JobAdvertisementsControllerTests()
        {
            _jobServiceMock = new Mock<IJobService>();
            _controller = new JobAdvertisementsController(_jobServiceMock.Object);
        }

        [Fact]
        public async Task GetJobAdvertisements_ShouldReturnOkWithData()
        {
            // Arrange
            _jobServiceMock.Setup(service => service.GetJobAdvertisementsAsync())
                .ReturnsAsync(new List<JobAdvertisementDto> { new JobAdvertisementDto() });

            // Act
            var result = await _controller.GetJobAdvertisements();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetJobAdvertisementsByUserId_ShouldReturnOkWithData()
        {
            //Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobAdvertisementsByUserIdAsync(userId))
                .ReturnsAsync(new List<JobAdvertisementDto> { new JobAdvertisementDto() });

            // Act
            var result = await _controller.GetJobAdvertisementsByUserId();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetJobAdvertisementsByUserId_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.GetJobAdvertisementsByUserId();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetJobsByUserId_ShouldReturnOkWithData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.GetJobAdvertisementsForUserAsync(userId))
                .ReturnsAsync(new List<JobAdvertisementDto> { new JobAdvertisementDto() });

            // Act
            var result = await _controller.GetJobsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetJobsByUserId_ShouldReturnNotFound_WhenNoJobsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.GetJobAdvertisementsForUserAsync(userId))
                .ReturnsAsync(new List<JobAdvertisementDto>()); // No jobs

            // Act
            var result = await _controller.GetJobsByUserId(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("No job advertisements found for this user.", apiResponse.Message);
        }

        [Fact]
        public async Task GetJobAdvertisementForm_ShouldReturnNotFound_WhenJobNotFound()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.GetJobAdvertisementFormByIdAsync(jobId))
                .ReturnsAsync((JobAdvertisementFormDto?)null); // Job not found

            // Act
            var result = await _controller.GetJobAdvertisementForm(jobId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Job advertisement not found.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateJobAdvertisement_ShouldReturnOk_WhenCreationIsSuccessful()
        {
            // Arrange
            var advertisementFormDto = new JobAdvertisementFormDto { Id = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.CreateJobAdvertisementAsync(advertisementFormDto, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateJobAdvertisement(advertisementFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Job advertisement created successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateJobAdvertisement_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.CreateJobAdvertisement(new JobAdvertisementFormDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateJobAdvertisement_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var advertisementFormDto = new JobAdvertisementFormDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var userId = advertisementFormDto.UserId;
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.UpdateJobAdvertisementAsync(advertisementFormDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateJobAdvertisement(advertisementFormDto.Id.Value, advertisementFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Job advertisement updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateJobAdvertisement_ShouldReturnUnauthorized_WhenUserIdIsMismatch()
        {
            // Arrange
            var advertisementFormDto = new JobAdvertisementFormDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            // Act
            var result = await _controller.UpdateJobAdvertisement(advertisementFormDto.Id.Value, advertisementFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to update this job advertisement.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateJobAdvertisement_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var advertisementFormDto = new JobAdvertisementFormDto { Id = Guid.NewGuid() }; // Different ID
            MockUser(_controller, Guid.NewGuid(), true);

            // Act
            var result = await _controller.UpdateJobAdvertisement(jobId, advertisementFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("ID mismatch.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateJobAdvertisement_ShouldReturnNotFound_WhenJobNotFound()
        {
            // Arrange
            var advertisementFormDto = new JobAdvertisementFormDto { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
            var userId = advertisementFormDto.UserId;
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.UpdateJobAdvertisementAsync(advertisementFormDto))
                .ReturnsAsync(false); // Simulate job not found

            // Act
            var result = await _controller.UpdateJobAdvertisement(advertisementFormDto.Id.Value, advertisementFormDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Job advertisement not found.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteJobAdvertisement_ShouldReturnOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.DeleteJobAdvertisementAsync(jobId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteJobAdvertisement(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Job advertisement deleted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteJobAdvertisement_ShouldReturnNotFound_WhenJobIsNotFound()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.DeleteJobAdvertisementAsync(jobId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteJobAdvertisement(jobId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Job advertisement not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetJobAdvertisementDetail_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.GetJobAdvertisementDetail(Guid.NewGuid());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetJobAdvertisementDetail_ShouldReturnNotFound_WhenJobNotFound()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobAdvertisementByIdAsync(jobId, userId))
                .ReturnsAsync((JobAdvertisementDetailDto?)null); // Job not found

            // Act
            var result = await _controller.GetJobAdvertisementDetail(jobId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Job advertisement not found.", apiResponse.Message);
        }
    }
}
