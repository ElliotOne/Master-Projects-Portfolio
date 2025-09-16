using Microsoft.AspNetCore.Identity;
using Moq;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Services;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.UserManagement.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IJwtHelper> _jwtHelperMock;
        private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();
            _jwtHelperMock = new Mock<IJwtHelper>();
            _blobStorageServiceMock = new Mock<IBlobStorageService>();

            // Use actual instance of JwtHelper with JwtSettings
            var jwtSettings = new JwtSettings
            {
                SecretKey = "your-secret-key",
                Issuer = "your-issuer",
                Audience = "your-audience",
                ExpiresInHours = 24
            };

            _userService = new UserService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _jwtHelperMock.Object,
                _blobStorageServiceMock.Object);
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<Role>> MockRoleManager()
        {
            var store = new Mock<IRoleStore<Role>>();
            return new Mock<RoleManager<Role>>(store.Object, null, null, null, null);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserIsCreated()
        {
            // Arrange
            var signUpDto = new BasicSignUpDto { Email = "testuser@example.com", Password = "Password123!" };
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), signUpDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.RegisterUserAsync(signUpDto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenUserCreationFails()
        {
            // Arrange
            var signUpDto = new BasicSignUpDto { Email = "testuser@example.com", Password = "Password123!" };
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), signUpDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _userService.RegisterUserAsync(signUpDto);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task CompleteUserRegistrationAsync_ShouldReturnSuccess_WhenRegistrationCompletes()
        {
            // Arrange
            var completeSignUpDto = new CompleteSignUpDto { Email = "testuser@example.com", RoleName = "User", Username = "testuser" };
            var user = new User { Email = completeSignUpDto.Email };

            _userManagerMock.Setup(um => um.FindByEmailAsync(completeSignUpDto.Email)).ReturnsAsync(user);
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(completeSignUpDto.RoleName)).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, completeSignUpDto.RoleName)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.CompleteUserRegistrationAsync(completeSignUpDto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task CompleteUserRegistrationAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var completeSignUpDto = new CompleteSignUpDto { Email = "nonexistent@example.com" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(completeSignUpDto.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.CompleteUserRegistrationAsync(completeSignUpDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("UserNotFound", result.Errors.First().Code);
        }

        [Fact]
        public async Task GenerateEmailConfirmationTokenAsync_ShouldReturnToken_WhenUserExists()
        {
            // Arrange
            var user = new User { UserName = "testuser" };
            _userManagerMock.Setup(um => um.FindByNameAsync("testuser")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("email-token");

            // Act
            var result = await _userService.GenerateEmailConfirmationTokenAsync("testuser");

            // Assert
            Assert.Equal("email-token", result);
        }

        [Fact]
        public async Task GenerateEmailConfirmationTokenAsync_ShouldReturnNull_WhenUserNotFound()
        {
            // Arrange
            _userManagerMock.Setup(um => um.FindByNameAsync("nonexistentuser")).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GenerateEmailConfirmationTokenAsync("nonexistentuser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var signInDto = new SignInDto
            {
                Email = "testuser@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = signInDto.Email,
                EmailConfirmed = true
            };

            var roles = new List<string> { "User" };  // Mocked roles
            var jwtToken = "mock-jwt-token";

            // Mock the necessary methods
            _userManagerMock.Setup(um => um.FindByEmailAsync(signInDto.Email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(user, signInDto.Password))
                .ReturnsAsync(true);

            _userManagerMock.Setup(um => um.GetRolesAsync(user))
                .ReturnsAsync(roles);

            _jwtHelperMock.Setup(jh => jh.GenerateToken(user, roles))
                .Returns(jwtToken);

            // Act
            var result = await _userService.LoginUserAsync(signInDto);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);  // Expect a JWT token as a string
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnNull_WhenLoginFails()
        {
            // Arrange
            var signInDto = new SignInDto { Email = "nonexistentuser@example.com", Password = "Password123!" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(signInDto.Email)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.LoginUserAsync(signInDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateProfileAsync_ShouldReturnSuccess_WhenProfileIsUpdated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var profileFormDto = new ProfileFormDto { UserId = userId, FirstName = "Ali", LastName = "Momenzadeh Kholenjani", PhoneNumber = "1234567890" };
            var user = new User { Id = userId };

            _userManagerMock.Setup(um => um.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.UpdateProfileAsync(profileFormDto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task UpdateProfileAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var profileFormDto = new ProfileFormDto { UserId = Guid.NewGuid() };

            _userManagerMock.Setup(um => um.FindByIdAsync(profileFormDto.UserId.ToString())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateProfileAsync(profileFormDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("User not found", result.Errors.First().Description);
        }

    }
}
