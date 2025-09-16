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
    public class GoalsControllerTests : TestBase
    {
        private readonly Mock<ITeamService> _teamServiceMock;
        private readonly Mock<ITeamAuthorizationService> _teamAuthorizationServiceMock;
        private readonly GoalsController _controller;

        public GoalsControllerTests()
        {
            _teamServiceMock = new Mock<ITeamService>();
            _teamAuthorizationServiceMock = new Mock<ITeamAuthorizationService>();
            _controller = new GoalsController(_teamServiceMock.Object, _teamAuthorizationServiceMock.Object);
        }

        [Fact]
        public async Task GetGoals_ShouldReturnOkWithGoals_WhenAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.GetGoalsByTeamIdAsync(teamId))
                .ReturnsAsync(new List<GoalDto> { new GoalDto() });

            // Act
            var result = await _controller.GetGoals(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetGoals_ShouldReturnUnauthorized_WhenNotAuthorized()
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
                })); // Authorization failed with object result

            // Act
            var result = await _controller.GetGoals(teamId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task GetGoalForm_ShouldReturnNotFound_WhenGoalNotFound()
        {
            // Arrange
            var goalId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetGoalFormByIdAsync(goalId))
                .ReturnsAsync((GoalFormDto?)null); // Goal not found

            // Act
            var result = await _controller.GetGoalForm(goalId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Goal not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetGoalForm_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var goalId = Guid.NewGuid();
            var goalFormDto = new GoalFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetGoalFormByIdAsync(goalId))
                .ReturnsAsync(goalFormDto); // Goal found

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(goalFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                })); // Authorization failed

            // Act
            var result = await _controller.GetGoalForm(goalId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateGoal_ShouldReturnOk_WhenGoalCreatedSuccessfully()
        {
            // Arrange
            var goalFormDto = new GoalFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(goalFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.CreateGoalAsync(goalFormDto))
                .ReturnsAsync(true); // Goal creation successful

            // Act
            var result = await _controller.CreateGoal(goalFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Goal created successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateGoal_ShouldReturnInternalServerError_WhenGoalCreationFails()
        {
            // Arrange
            var goalFormDto = new GoalFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(goalFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.CreateGoalAsync(goalFormDto))
                .ReturnsAsync(false); // Goal creation failed

            // Act
            var result = await _controller.CreateGoal(goalFormDto);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<object>>(errorResult.Value);
            Assert.Equal("An error occurred while creating the goal.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateGoal_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var goalFormDto = new GoalFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(goalFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                })); // Authorization failed

            // Act
            var result = await _controller.CreateGoal(goalFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateGoal_ShouldReturnOk_WhenGoalUpdatedSuccessfully()
        {
            // Arrange
            var goalFormDto = new GoalFormDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(goalFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.UpdateGoalAsync(goalFormDto))
                .ReturnsAsync(true); // Goal update successful

            // Act
            var result = await _controller.UpdateGoal(goalFormDto.Id.Value, goalFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Goal updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateGoal_ShouldReturnNotFound_WhenGoalNotFound()
        {
            // Arrange
            var goalFormDto = new GoalFormDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(goalFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.UpdateGoalAsync(goalFormDto))
                .ReturnsAsync(false); // Goal not found

            // Act
            var result = await _controller.UpdateGoal(goalFormDto.Id.Value, goalFormDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Goal not found.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateGoal_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var routeGoalId = Guid.NewGuid();
            var goalFormDto = new GoalFormDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() }; // Different ID
            MockUser(_controller, Guid.NewGuid(), true);

            // Act
            var result = await _controller.UpdateGoal(routeGoalId, goalFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("ID mismatch.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteGoal_ShouldReturnOk_WhenGoalDeletedSuccessfully()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.DeleteGoalAsync(goalId))
                .ReturnsAsync(true); // Goal deleted successfully

            // Act
            var result = await _controller.DeleteGoal(teamId, goalId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Goal deleted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteGoal_ShouldReturnNotFound_WhenGoalNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult()); // Authorization successful

            _teamServiceMock.Setup(service => service.DeleteGoalAsync(goalId))
                .ReturnsAsync(false); // Goal not found

            // Act
            var result = await _controller.DeleteGoal(teamId, goalId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Goal not found.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteGoal_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var goalId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                })); // Authorization failed

            // Act
            var result = await _controller.DeleteGoal(teamId, goalId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }
    }
}
