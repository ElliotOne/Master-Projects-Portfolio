using Microsoft.AspNetCore.Mvc;
using Moq;
using StartupTeam.Module.TeamManagement.Controllers;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Tests.Shared;
using System.Security.Claims;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.TeamManagement.Controllers
{
    public class MilestonesControllerTests : TestBase
    {
        private readonly Mock<ITeamService> _teamServiceMock;
        private readonly Mock<ITeamAuthorizationService> _teamAuthorizationServiceMock;
        private readonly MilestonesController _controller;

        public MilestonesControllerTests()
        {
            _teamServiceMock = new Mock<ITeamService>();
            _teamAuthorizationServiceMock = new Mock<ITeamAuthorizationService>();
            _controller = new MilestonesController(_teamServiceMock.Object, _teamAuthorizationServiceMock.Object);
        }

        [Fact]
        public async Task GetMilestones_ShouldReturnOkWithMilestones_WhenAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.GetMilestonesByTeamIdAsync(teamId))
                .ReturnsAsync(new List<MilestoneDto> { new MilestoneDto() });

            // Act
            var result = await _controller.GetMilestones(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetMilestones_ShouldReturnUnauthorized_WhenNotAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.GetMilestones(teamId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task GetMilestoneForm_ShouldReturnOkWithData_WhenMilestoneIsFound()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var milestoneFromDto = new MilestoneFromDto { Id = milestoneId, TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetMilestoneFormByIdAsync(milestoneId))
                .ReturnsAsync(milestoneFromDto);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _controller.GetMilestoneForm(milestoneId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<MilestoneFromDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetMilestoneForm_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var milestoneId = Guid.NewGuid();
            var milestoneFromDto = new MilestoneFromDto { Id = milestoneId, TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetMilestoneFormByIdAsync(milestoneId))
                .ReturnsAsync(milestoneFromDto);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.GetMilestoneForm(milestoneId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateMilestone_ShouldReturnOk_WhenMilestoneCreatedSuccessfully()
        {
            // Arrange
            var milestoneFromDto = new MilestoneFromDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsMilestoneDueDateValid(milestoneFromDto))
                .ReturnsAsync(true);

            _teamServiceMock.Setup(service => service.CreateMilestoneAsync(milestoneFromDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateMilestone(milestoneFromDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Milestone created successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateMilestone_ShouldReturnInternalServerError_WhenMilestoneCreationFails()
        {
            // Arrange
            var milestoneFromDto = new MilestoneFromDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsMilestoneDueDateValid(milestoneFromDto))
                .ReturnsAsync(true);

            _teamServiceMock.Setup(service => service.CreateMilestoneAsync(milestoneFromDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreateMilestone(milestoneFromDto);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<object>>(errorResult.Value);
            Assert.Equal("An error occurred while creating the milestone.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateMilestone_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var milestoneFromDto = new MilestoneFromDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.CreateMilestone(milestoneFromDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateMilestone_ShouldReturnOk_WhenMilestoneUpdatedSuccessfully()
        {
            // Arrange
            var milestoneFromDto = new MilestoneFromDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsMilestoneDueDateValid(milestoneFromDto))
                .ReturnsAsync(true);

            _teamServiceMock.Setup(service => service.UpdateMilestoneAsync(milestoneFromDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateMilestone(milestoneFromDto.Id.Value, milestoneFromDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Milestone updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateMilestone_ShouldReturnNotFound_WhenMilestoneNotFound()
        {
            // Arrange
            var milestoneFromDto = new MilestoneFromDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(milestoneFromDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsMilestoneDueDateValid(milestoneFromDto))
                .ReturnsAsync(true);

            _teamServiceMock.Setup(service => service.UpdateMilestoneAsync(milestoneFromDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateMilestone(milestoneFromDto.Id.Value, milestoneFromDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Milestone not found.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateMilestone_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var routeMilestoneId = Guid.NewGuid();
            var milestoneFromDto = new MilestoneFromDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() }; // Different Id
            MockUser(_controller, Guid.NewGuid(), true);

            // Act
            var result = await _controller.UpdateMilestone(routeMilestoneId, milestoneFromDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("ID mismatch.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteMilestone_ShouldReturnOk_WhenMilestoneDeletedSuccessfully()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var milestoneId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteMilestoneAsync(milestoneId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteMilestone(teamId, milestoneId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Milestone deleted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteMilestone_ShouldReturnNotFound_WhenMilestoneNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var milestoneId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteMilestoneAsync(milestoneId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteMilestone(teamId, milestoneId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Milestone not found.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteMilestone_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var milestoneId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.DeleteMilestone(teamId, milestoneId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }
    }
}
