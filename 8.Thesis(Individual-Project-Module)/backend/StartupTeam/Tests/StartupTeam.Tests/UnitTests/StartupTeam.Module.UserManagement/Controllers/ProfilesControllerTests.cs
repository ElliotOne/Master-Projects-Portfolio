using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StartupTeam.Module.UserManagement.Controllers;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Tests.Shared;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.UserManagement.Controllers
{
    public class ProfilesControllerTests : TestBase
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly ProfilesController _controller;

        public ProfilesControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new ProfilesController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetProfiles_ShouldReturnOkWithData()
        {
            // Arrange
            _userServiceMock.Setup(service => service.GetProfilesAsync())
                .ReturnsAsync(new List<ProfileDto> { new ProfileDto() });

            // Act
            var result = await _controller.GetProfiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetProfiles_ShouldReturnEmptyList_WhenNoProfilesExist()
        {
            // Arrange
            _userServiceMock.Setup(service => service.GetProfilesAsync())
                .ReturnsAsync(new List<ProfileDto>()); // Empty list

            // Act
            var result = await _controller.GetProfiles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            var profiles = Assert.IsType<List<ProfileDto>>(apiResponse.Data);
            Assert.Empty(profiles);
        }

        [Fact]
        public async Task GetProfileDetail_ShouldReturnOkWithProfile_WhenProfileExists()
        {
            // Arrange
            var username = "testuser";
            _userServiceMock.Setup(service => service.GetProfileByUsernameAsync(username))
                .ReturnsAsync(new ProfileDetailDto { UserName = username });

            // Act
            var result = await _controller.GetProfileDetail(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<ProfileDetailDto>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetProfileDetail_ShouldReturnNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var username = "nonexistentuser";
            _userServiceMock.Setup(service => service.GetProfileByUsernameAsync(username))
                .ReturnsAsync((ProfileDetailDto?)null); // Profile not found

            // Act
            var result = await _controller.GetProfileDetail(username);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Profile not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetProfileForm_ShouldReturnOkWithProfileForm_WhenUserIdIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _userServiceMock.Setup(service => service.GetProfileFormByUserIdAsync(userId))
                .ReturnsAsync(new ProfileFormDto());

            // Act
            var result = await _controller.GetProfileForm();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task GetProfileForm_ShouldReturnBadRequest_WhenUserIdIsMissing()
        {
            // Arrange
            MockUser(_controller, null, false);

            // Act
            var result = await _controller.GetProfileForm();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task GetProfileForm_ShouldReturnNotFound_WhenProfileDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            _userServiceMock.Setup(service => service.GetProfileFormByUserIdAsync(userId))
                .ReturnsAsync((ProfileFormDto?)null); // Profile not found

            // Act
            var result = await _controller.GetProfileForm();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Profile not found.", apiResponse.Message);
        }

        [Fact]
        public async Task GetProfileForm_ShouldReturnBadRequest_WhenUserNotAuthenticated()
        {
            // Arrange
            MockUser(_controller, null, false); // User is not authenticated

            // Act
            var result = await _controller.GetProfileForm();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User ID is missing or invalid.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "testuser";
            MockUser(_controller, userId, true);

            var profileFormDto = new ProfileFormDto { UserName = userName, UserId = userId };

            _userServiceMock.Setup(service => service.UpdateProfileAsync(profileFormDto))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.UpdateProfile(userName, profileFormDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("Profile updated successfully.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnBadRequest_WhenUserNameMismatch()
        {
            // Arrange
            var userName = "testuser";
            var profileFormDto = new ProfileFormDto { UserName = "otheruser" };

            // Act
            var result = await _controller.UpdateProfile(userName, profileFormDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("UserName mismatch.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnUnauthorized_WhenUserIdMismatch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profileFormDto = new ProfileFormDto { UserId = Guid.NewGuid(), UserName = "testuser" };
            MockUser(_controller, userId, true);

            // Act
            var result = await _controller.UpdateProfile(profileFormDto.UserName, profileFormDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("You are not authorized to update this profile.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnNotFound_WhenProfileNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            MockUser(_controller, userId, true);

            var profileFormDto = new ProfileFormDto { UserName = "testuser", UserId = userId };

            _userServiceMock.Setup(service => service.UpdateProfileAsync(profileFormDto))
                .ReturnsAsync(IdentityResult.Failed()); // Update failed

            // Act
            var result = await _controller.UpdateProfile(profileFormDto.UserName, profileFormDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Profile not found.", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnNotFound_WhenUpdateFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "testuser";
            MockUser(_controller, userId, true);

            var profileFormDto = new ProfileFormDto { UserName = userName, UserId = userId };

            // Simulate an update failure by returning IdentityResult.Failed
            _userServiceMock.Setup(service => service.UpdateProfileAsync(profileFormDto))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            // Act
            var result = await _controller.UpdateProfile(userName, profileFormDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(notFoundResult.Value);
            Assert.Equal("Profile not found.", apiResponse.Message);
        }
    }
}
