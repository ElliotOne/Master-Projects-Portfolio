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
    public class TeamRolesControllerTests : TestBase
    {
        private readonly Mock<ITeamService> _teamServiceMock;
        private readonly Mock<ITeamAuthorizationService> _teamAuthorizationServiceMock;
        private readonly TeamRolesController _controller;

        public TeamRolesControllerTests()
        {
            _teamServiceMock = new Mock<ITeamService>();
            _teamAuthorizationServiceMock = new Mock<ITeamAuthorizationService>();
            _controller = new TeamRolesController(_teamServiceMock.Object, _teamAuthorizationServiceMock.Object);
        }

        [Fact]
        public async Task GetTeamRoles_ShouldReturnOkWithRoles_WhenAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.GetTeamRolesByTeamIdAsync(teamId))
                .ReturnsAsync(new List<TeamRoleDto> { new TeamRoleDto() });

            // Act
            var result = await _controller.GetTeamRoles(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamRoles_ShouldReturnUnauthorized_WhenNotAuthorized()
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
            var result = await _controller.GetTeamRoles(teamId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTeamRoleForm_ShouldReturnOkWithRole_WhenRoleIsFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            var teamRoleFormDto = new TeamRoleFormDto { Id = roleId, TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetTeamRoleFormByIdAsync(roleId))
                .ReturnsAsync(teamRoleFormDto);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _controller.GetTeamRoleForm(roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<TeamRoleFormDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamRoleForm_ShouldReturnNotFound_WhenRoleIsNotFound()
        {
            // Arrange
            var roleId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetTeamRoleFormByIdAsync(roleId))
                .ReturnsAsync((TeamRoleFormDto?)null);

            // Act
            var result = await _controller.GetTeamRoleForm(roleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Role not found.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateTeamRole_ShouldReturnOk_WhenTeamRoleCreatedSuccessfully()
        {
            // Arrange
            var teamRoleFormDto = new TeamRoleFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.CreateTeamRoleAsync(teamRoleFormDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CreateTeamRole(teamRoleFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team role created successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateTeamRole_ShouldReturnInternalServerError_WhenTeamRoleCreationFails()
        {
            // Arrange
            var teamRoleFormDto = new TeamRoleFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.CreateTeamRoleAsync(teamRoleFormDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreateTeamRole(teamRoleFormDto);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, errorResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<object>>(errorResult.Value);
            Assert.Equal("An error occurred while creating the team role.", apiResponse.Message);
        }

        [Fact]
        public async Task CreateTeamRole_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamRoleFormDto = new TeamRoleFormDto { TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.CreateTeamRole(teamRoleFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeamRole_ShouldReturnOk_WhenTeamRoleUpdatedSuccessfully()
        {
            // Arrange
            var teamRoleFormDto = new TeamRoleFormDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.UpdateTeamRoleAsync(teamRoleFormDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTeamRole(teamRoleFormDto.Id.Value, teamRoleFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team role updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeamRole_ShouldReturnNotFound_WhenTeamRoleNotFound()
        {
            // Arrange
            var teamRoleFormDto = new TeamRoleFormDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.UpdateTeamRoleAsync(teamRoleFormDto))
                .ReturnsAsync(false); // Team role not found

            // Act
            var result = await _controller.UpdateTeamRole(teamRoleFormDto.Id.Value, teamRoleFormDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team role not found.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeamRole_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamRoleFormDto = new TeamRoleFormDto { Id = Guid.NewGuid(), TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamRoleFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.UpdateTeamRole(teamRoleFormDto.Id.Value, teamRoleFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteTeamRole_ShouldReturnOk_WhenTeamRoleDeletedSuccessfully()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteTeamRoleAsync(roleId))
                .ReturnsAsync(true); // Team role deleted successfully

            // Act
            var result = await _controller.DeleteTeamRole(teamId, roleId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team role deleted successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteTeamRole_ShouldReturnNotFound_WhenTeamRoleNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var roleId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteTeamRoleAsync(roleId))
                .ReturnsAsync(false); // Team role not found

            // Act
            var result = await _controller.DeleteTeamRole(teamId, roleId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team role not found.", apiResponse.Message);
        }
    }
}
