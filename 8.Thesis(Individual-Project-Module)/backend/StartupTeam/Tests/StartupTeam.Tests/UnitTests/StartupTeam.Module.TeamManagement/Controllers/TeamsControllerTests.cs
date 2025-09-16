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
    public class TeamsControllerTests : TestBase
    {
        private readonly Mock<ITeamService> _teamServiceMock;
        private readonly Mock<ITeamAuthorizationService> _teamAuthorizationServiceMock;
        private readonly TeamsController _controller;

        public TeamsControllerTests()
        {
            _teamServiceMock = new Mock<ITeamService>();
            _teamAuthorizationServiceMock = new Mock<ITeamAuthorizationService>();
            _controller = new TeamsController(_teamServiceMock.Object, _teamAuthorizationServiceMock.Object);
        }

        [Fact]
        public async Task GetIndividualTeams_ShouldReturnOkWithTeams_WhenUserIdIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _teamServiceMock.Setup(service => service.GetTeamsByIndividualIdAsync(userId))
                .ReturnsAsync(new List<TeamDto> { new TeamDto() });

            // Act
            var result = await _controller.GetIndividualTeams();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetIndividualTeams_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.GetIndividualTeams();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetFounderTeams_ShouldReturnOkWithTeams_WhenUserIdIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _teamServiceMock.Setup(service => service.GetTeamsByFounderIdAsync(userId))
                .ReturnsAsync(new List<TeamDto> { new TeamDto() });

            // Act
            var result = await _controller.GetFounderTeams();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamForm_ShouldReturnOkWithTeamForm_WhenAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            var teamFormDto = new TeamFormDto { Id = teamId };
            _teamServiceMock.Setup(service => service.GetTeamFormByIdAsync(teamId))
                .ReturnsAsync(teamFormDto);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _controller.GetTeamForm(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<TeamFormDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamForm_ShouldReturnNotFound_WhenTeamNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetTeamFormByIdAsync(teamId))
                .ReturnsAsync((TeamFormDto?)null);

            // Act
            var result = await _controller.GetTeamForm(teamId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTeamDetail_ShouldReturnOkWithTeamDetail_WhenAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.GetTeamByIdAsync(teamId))
                .ReturnsAsync(new TeamDetailDto() { Id = teamId });

            // Act
            var result = await _controller.GetTeamDetail(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<TeamDetailDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamDetail_ShouldReturnNotFound_WhenTeamNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetTeamByIdAsync(teamId))
                .ReturnsAsync((TeamDetailDto?)null); // No team found

            // Act
            var result = await _controller.GetTeamDetail(teamId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTeamDetail_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var teamDetailDto = new TeamDetailDto { Id = teamId, Name = "Test Team" };

            // Mock that the team exists
            _teamServiceMock.Setup(service => service.GetTeamByIdAsync(teamId))
                .ReturnsAsync(teamDetailDto);

            MockUser(_controller, Guid.NewGuid(), true);

            // Mock authorization failure
            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.GetTeamDetail(teamId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateTeam_ShouldReturnOk_WhenTeamCreatedSuccessfully()
        {
            // Arrange
            var teamFormDto = new TeamFormDto();
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _teamServiceMock.Setup(service => service.CreateTeamAsync(teamFormDto))
                .ReturnsAsync(true); // Team created successfully

            // Act
            var result = await _controller.CreateTeam(teamFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team created successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateTeam_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.CreateTeam(new TeamFormDto());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeam_ShouldReturnOk_WhenTeamUpdatedSuccessfully()
        {
            // Arrange
            var teamFormDto = new TeamFormDto { Id = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamFormDto.Id!.Value, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.UpdateTeamAsync(teamFormDto))
                .ReturnsAsync(true); // Team updated successfully

            // Act
            var result = await _controller.UpdateTeam(teamFormDto.Id!.Value, teamFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeam_ShouldReturnNotFound_WhenTeamNotFound()
        {
            // Arrange
            var teamFormDto = new TeamFormDto { Id = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamFormDto.Id!.Value, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.UpdateTeamAsync(teamFormDto))
                .ReturnsAsync(false); // Team not found

            // Act
            var result = await _controller.UpdateTeam(teamFormDto.Id!.Value, teamFormDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team not found.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteTeam_ShouldReturnOk_WhenTeamDeletedSuccessfully()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteTeamAsync(teamId))
                .ReturnsAsync(true); // Team deleted successfully

            // Act
            var result = await _controller.DeleteTeam(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team deleted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteTeam_ShouldReturnNotFound_WhenTeamNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteTeamAsync(teamId))
                .ReturnsAsync(false); // Team not found

            // Act
            var result = await _controller.DeleteTeam(teamId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team not found.", apiResponse.Message);
        }
    }
}
