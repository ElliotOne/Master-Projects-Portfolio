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
    public class TeamMembersControllerTests : TestBase
    {
        private readonly Mock<ITeamService> _teamServiceMock;
        private readonly Mock<ITeamAuthorizationService> _teamAuthorizationServiceMock;
        private readonly TeamMembersController _controller;

        public TeamMembersControllerTests()
        {
            _teamServiceMock = new Mock<ITeamService>();
            _teamAuthorizationServiceMock = new Mock<ITeamAuthorizationService>();
            _controller = new TeamMembersController(_teamServiceMock.Object, _teamAuthorizationServiceMock.Object);
        }

        [Fact]
        public async Task GetTeamMembers_ShouldReturnOkWithMembers_WhenAuthorized()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.GetTeamMembersByTeamIdAsync(teamId))
                .ReturnsAsync(new List<TeamMemberDto> { new TeamMemberDto() });

            // Act
            var result = await _controller.GetTeamMembers(teamId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamMembers_ShouldReturnUnauthorized_WhenNotAuthorized()
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
            var result = await _controller.GetTeamMembers(teamId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task GetTeamMemberForm_ShouldReturnOkWithTeamMember_WhenTeamMemberIsFound()
        {
            // Arrange
            var teamMemberId = Guid.NewGuid();
            var teamMemberFormDto = new TeamMemberFormDto { Id = teamMemberId, TeamId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetTeamMemberFormByIdAsync(teamMemberId))
                .ReturnsAsync(teamMemberFormDto);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _controller.GetTeamMemberForm(teamMemberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<TeamMemberFormDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetTeamMemberForm_ShouldReturnNotFound_WhenTeamMemberIsNotFound()
        {
            // Arrange
            var teamMemberId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamServiceMock.Setup(service => service.GetTeamMemberFormByIdAsync(teamMemberId))
                .ReturnsAsync((TeamMemberFormDto?)null);

            // Act
            var result = await _controller.GetTeamMemberForm(teamMemberId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team member not found.", apiResponse.Message);
        }

        [Fact]
        public async Task AddTeamMember_ShouldReturnOk_WhenTeamMemberAddedSuccessfully()
        {
            // Arrange
            var teamMemberFormDto = new TeamMemberFormDto
            { TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), TeamRoleId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsIndividualInTeamWithRole(
                    teamMemberFormDto.TeamId, teamMemberFormDto.UserId, teamMemberFormDto.TeamRoleId))
                .ReturnsAsync(false);

            _teamServiceMock.Setup(service => service.CreateTeamMemberAsync(teamMemberFormDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.AddTeamMember(teamMemberFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team member added successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task AddTeamMember_ShouldReturnBadRequest_WhenIndividualAlreadyInTeamWithSameRole()
        {
            // Arrange
            var teamMemberFormDto = new TeamMemberFormDto
            { TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), TeamRoleId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsIndividualInTeamWithRole(
                    teamMemberFormDto.TeamId, teamMemberFormDto.UserId, teamMemberFormDto.TeamRoleId))
                .ReturnsAsync(true); // Individual already in team with the same role

            // Act
            var result = await _controller.AddTeamMember(teamMemberFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Individual is already in the team with the same role.", apiResponse.Message);
        }

        [Fact]
        public async Task AddTeamMember_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamMemberFormDto = new TeamMemberFormDto
            { TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), TeamRoleId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.AddTeamMember(teamMemberFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeamMember_ShouldReturnOk_WhenTeamMemberUpdatedSuccessfully()
        {
            // Arrange
            var teamMemberFormDto = new TeamMemberFormDto
            { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), TeamRoleId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsIndividualInTeamWithRole(
                    teamMemberFormDto.TeamId, teamMemberFormDto.UserId, teamMemberFormDto.TeamRoleId))
                .ReturnsAsync(false);

            _teamServiceMock.Setup(service => service.UpdateTeamMemberAsync(teamMemberFormDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateTeamMember(teamMemberFormDto.Id.Value, teamMemberFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team member updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeamMember_ShouldReturnBadRequest_WhenIndividualAlreadyInTeamWithSameRole()
        {
            // Arrange
            var teamMemberFormDto = new TeamMemberFormDto
            { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), TeamRoleId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.IsIndividualInTeamWithRole(
                    teamMemberFormDto.TeamId, teamMemberFormDto.UserId, teamMemberFormDto.TeamRoleId))
                .ReturnsAsync(true); // Individual already in team with the same role

            // Act
            var result = await _controller.UpdateTeamMember(teamMemberFormDto.Id.Value, teamMemberFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Individual is already in the team with the same role.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateTeamMember_ShouldReturnUnauthorized_WhenAuthorizationFails()
        {
            // Arrange
            var teamMemberFormDto = new TeamMemberFormDto
            { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), UserId = Guid.NewGuid(), TeamRoleId = Guid.NewGuid() };
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamMemberFormDto.TeamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                }));

            // Act
            var result = await _controller.UpdateTeamMember(teamMemberFormDto.Id.Value, teamMemberFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to perform this action.", apiResponse.Message);
        }

        [Fact]
        public async Task RemoveTeamMember_ShouldReturnOk_WhenTeamMemberRemovedSuccessfully()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var teamMemberId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteTeamMemberAsync(teamMemberId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.RemoveTeamMember(teamId, teamMemberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Team member removed successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task RemoveTeamMember_ShouldReturnNotFound_WhenTeamMemberNotFound()
        {
            // Arrange
            var teamId = Guid.NewGuid();
            var teamMemberId = Guid.NewGuid();
            MockUser(_controller, Guid.NewGuid(), true);

            _teamAuthorizationServiceMock
                .Setup(service => service.AuthorizeTeamAction(teamId, It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new OkResult());

            _teamServiceMock.Setup(service => service.DeleteTeamMemberAsync(teamMemberId))
                .ReturnsAsync(false); // Team member not found

            // Act
            var result = await _controller.RemoveTeamMember(teamId, teamMemberId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Team member not found.", apiResponse.Message);
        }
    }
}