using Microsoft.AspNetCore.Mvc;
using Moq;
using StartupTeam.Module.JobManagement.Controllers;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Models.Enums;
using StartupTeam.Module.JobManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Tests.Shared;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.JobManagement.Controllers
{
    public class JobApplicationsControllerTests : TestBase
    {
        private readonly Mock<IJobService> _jobServiceMock;
        private readonly JobApplicationsController _controller;

        public JobApplicationsControllerTests()
        {
            _jobServiceMock = new Mock<IJobService>();
            _controller = new JobApplicationsController(_jobServiceMock.Object);
        }

        [Fact]
        public async Task GetIndividualJobApplications_ShouldReturnOkWithData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);
            _jobServiceMock.Setup(service => service.GetJobApplicationsByIndividualIdAsync(userId))
                .ReturnsAsync(new List<JobApplicationDto> { new JobApplicationDto() });

            // Act
            var result = await _controller.GetIndividualJobApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetIndividualJobApplications_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.GetIndividualJobApplications();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetFounderJobApplications_ShouldReturnOkWithData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);
            _jobServiceMock.Setup(service => service.GetJobApplicationsByFounderIdAsync(userId, null))
                .ReturnsAsync(new List<JobApplicationDto> { new JobApplicationDto() });

            // Act
            var result = await _controller.GetFounderJobApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetFounderJobApplications_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.GetFounderJobApplications();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetSuccessfulJobApplicants_ShouldReturnOkWithData()
        {
            // Arrange
            var jobId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.GetSuccessfulJobApplicantsByJobAdvertisementIdAsync(jobId))
                .ReturnsAsync(new List<JobApplicantDto> { new JobApplicantDto() });

            // Act
            var result = await _controller.GetSuccessfulJobApplicants(jobId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task SubmitIndividualJobApplication_ShouldReturnOk_WhenSubmissionIsSuccessful()
        {
            // Arrange
            var applicationFormDto = new JobApplicationFormDto { JobAdvertisementId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobAdvertisementByIdAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(() => new JobAdvertisementDetailDto());

            _jobServiceMock.Setup(service => service.HasUserAlreadyAppliedAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(false);

            _jobServiceMock.Setup(service => service.SubmitJobApplicationAsync(applicationFormDto, userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.SubmitIndividualJobApplication(applicationFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Job application submitted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task SubmitIndividualJobApplication_ShouldReturnBadRequest_WhenUserHasAlreadyApplied()
        {
            // Arrange
            var applicationFormDto = new JobApplicationFormDto { JobAdvertisementId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobAdvertisementByIdAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(() => new JobAdvertisementDetailDto() { Id = applicationFormDto.JobAdvertisementId });

            _jobServiceMock.Setup(service => service.HasUserAlreadyAppliedAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(() => true);

            // Act
            var result = await _controller.SubmitIndividualJobApplication(applicationFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("You have already applied for this job.", apiResponse.Message);
        }

        [Fact]
        public async Task SubmitIndividualJobApplication_ShouldReturnBadRequest_WhenJobAdvertisementNotFound()
        {
            // Arrange
            var applicationFormDto = new JobApplicationFormDto { JobAdvertisementId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobAdvertisementByIdAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(() => (JobAdvertisementDetailDto?)null); // No job found

            // Act
            var result = await _controller.SubmitIndividualJobApplication(applicationFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Job advertisement not found.", apiResponse.Message);
        }

        [Fact]
        public async Task SubmitIndividualJobApplication_ShouldReturnBadRequest_WhenCVIsRequiredButNotProvided()
        {
            // Arrange
            var applicationFormDto = new JobApplicationFormDto { JobAdvertisementId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            var jobAdvertisement = new JobAdvertisementDetailDto { Id = applicationFormDto.JobAdvertisementId, RequireCV = true };

            _jobServiceMock.Setup(service => service.GetJobAdvertisementByIdAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(jobAdvertisement);

            _jobServiceMock.Setup(service => service.HasUserAlreadyAppliedAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SubmitIndividualJobApplication(applicationFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("CV is required for this job application.", apiResponse.Message);
        }

        [Fact]
        public async Task SubmitIndividualJobApplication_ShouldReturnBadRequest_WhenCoverLetterIsRequiredButNotProvided()
        {
            // Arrange
            var applicationFormDto = new JobApplicationFormDto { JobAdvertisementId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            var jobAdvertisement = new JobAdvertisementDetailDto { Id = applicationFormDto.JobAdvertisementId, RequireCoverLetter = true };

            _jobServiceMock.Setup(service => service.GetJobAdvertisementByIdAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(jobAdvertisement);

            _jobServiceMock.Setup(service => service.HasUserAlreadyAppliedAsync(applicationFormDto.JobAdvertisementId, userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.SubmitIndividualJobApplication(applicationFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Cover letter is required for this job application.", apiResponse.Message);
        }

        [Fact]
        public async Task GetJobApplicationDetails_ShouldReturnOkWithData()
        {
            // Arrange
            var jobAppId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.GetJobApplicationFormByIdAsync(jobAppId))
                .ReturnsAsync(() => new JobApplicationUpdateFormDto());

            // Act
            var result = await _controller.GetJobApplicationDetails(jobAppId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetJobApplicationDetails_ShouldReturnNotFound_WhenJobApplicationNotFound()
        {
            // Arrange
            var jobAppId = Guid.NewGuid();
            _jobServiceMock.Setup(service => service.GetJobApplicationFormByIdAsync(jobAppId))
                .ReturnsAsync(() => (JobApplicationUpdateFormDto?)null);

            // Act
            var result = await _controller.GetJobApplicationDetails(jobAppId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Job application not found.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateJobApplication_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var jobAppUpdateDto = new JobApplicationUpdateFormDto { Id = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _jobServiceMock.Setup(service => service.UpdateJobApplicationAsync(jobAppUpdateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateJobApplication(jobAppUpdateDto.Id, jobAppUpdateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Job application status updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateJobApplication_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var jobAppUpdateDto = new JobApplicationUpdateFormDto { Id = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            // Act
            var result = await _controller.UpdateJobApplication(Guid.NewGuid(), jobAppUpdateDto); // Mismatched IDs

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("ID mismatch.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ShouldReturnOk_WhenStatusUpdatedSuccessfully()
        {
            // Arrange
            var jobAppUpdateDto = new JobApplicationUpdateFormDto { Status = JobApplicationStatus.OfferAcceptedByIndividual };
            var jobAppId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobApplicationByIdAsync(jobAppId))
                .ReturnsAsync(new JobApplicationDetailDto { UserId = userId });

            _jobServiceMock.Setup(service => service.UpdateApplicationStatusByIndividualAsync(jobAppId, jobAppUpdateDto.Status))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateApplicationStatus(jobAppId, jobAppUpdateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Application status updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
        {
            // Arrange
            var jobAppUpdateDto = new JobApplicationUpdateFormDto { Status = JobApplicationStatus.OfferAcceptedByIndividual };
            var jobAppId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid(); // Different user
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobApplicationByIdAsync(jobAppId))
                .ReturnsAsync(new JobApplicationDetailDto { UserId = otherUserId }); // Belongs to another user

            // Act
            var result = await _controller.UpdateApplicationStatus(jobAppId, jobAppUpdateDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to update this job application.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateApplicationStatus_ShouldReturnBadRequest_WhenStatusUpdateFails()
        {
            // Arrange
            var jobAppUpdateDto = new JobApplicationUpdateFormDto { Status = JobApplicationStatus.OfferAcceptedByIndividual };
            var jobAppId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _jobServiceMock.Setup(service => service.GetJobApplicationByIdAsync(jobAppId))
                .ReturnsAsync(new JobApplicationDetailDto { UserId = userId });

            _jobServiceMock.Setup(service => service.UpdateApplicationStatusByIndividualAsync(jobAppId, jobAppUpdateDto.Status))
                .ReturnsAsync(false); // Simulate failure

            // Act
            var result = await _controller.UpdateApplicationStatus(jobAppId, jobAppUpdateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Unable to update the application status.", apiResponse.Message);
        }

    }
}
