using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Moq;
using StartupTeam.Module.UserManagement.Controllers;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Shared.Services;

namespace StartupTeam.Tests.UnitTests.StartupTeam.Module.UserManagement.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<IGoogleAuthHelper> _googleAuthHelperMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _mailServiceMock = new Mock<IMailService>();
            _googleAuthHelperMock = new Mock<IGoogleAuthHelper>();
            var config = new Mock<IConfiguration>();
            config.Setup(c => c["ConfirmEmailRedirectUrl"]).Returns("http://localhost:3000/confirm-email?token=");
            _controller = new AuthController(_userServiceMock.Object, _mailServiceMock.Object, _googleAuthHelperMock.Object, config.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerName = "Auth",
                    ActionName = "CompleteSignUp"
                }
            };
        }

        [Fact]
        public async Task SignUp_ShouldReturnOk_WhenSignUpIsSuccessful()
        {
            // Arrange
            var basicSignUpDto = new BasicSignUpDto { Email = "testuser@example.com", Password = "Password123!" };
            _userServiceMock.Setup(service => service.RegisterUserAsync(basicSignUpDto))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.SignUp(basicSignUpDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.NotNull(apiResponse.Data);
        }

        [Fact]
        public async Task SignUp_ShouldReturnBadRequest_WhenSignUpFails()
        {
            // Arrange
            var basicSignUpDto = new BasicSignUpDto { Email = "testuser@example.com", Password = "Password123!" };
            var identityErrors = new List<IdentityError> { new IdentityError { Code = "DuplicateUserName", Description = "Email already exists" } };

            _userServiceMock.Setup(service => service.RegisterUserAsync(basicSignUpDto))
                .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            // Act
            var result = await _controller.SignUp(basicSignUpDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.False(apiResponse.Success);

            // Cast Errors to a dictionary and assert its content
            var errorsDictionary = Assert.IsType<Dictionary<string, List<string>>>(apiResponse.Errors);
            Assert.Contains("DuplicateUserName", errorsDictionary.Keys);
        }

        [Fact]
        public async Task CompleteSignUp_ShouldReturnOk_WhenSignUpIsSuccessful()
        {
            // Arrange
            var completeSignUpDto = new CompleteSignUpDto { Email = "testuser@example.com", Username = "testuser", ExternalSignIn = false };

            _userServiceMock.Setup(service => service.CompleteUserRegistrationAsync(completeSignUpDto))
                .ReturnsAsync(IdentityResult.Success);

            _userServiceMock.Setup(service => service.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new User { Email = "testuser@example.com" });

            _userServiceMock.Setup(service => service.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("email-token");

            _mailServiceMock.Setup(service => service.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Mock the IUrlHelper
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/confirm-email");

            // Set the mocked IUrlHelper in the controller
            _controller.Url = urlHelperMock.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerName = "Auth",
                    ActionName = "CompleteSignUp"
                }
            };

            // Act
            var result = await _controller.CompleteSignUp(completeSignUpDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(apiResponse.Success);
        }

        [Fact]
        public async Task CompleteSignUp_ShouldReturnBadRequest_WhenTokenGenerationFails()
        {
            // Arrange
            var completeSignUpDto = new CompleteSignUpDto { Email = "testuser@example.com", Username = "testuser", ExternalSignIn = false };

            _userServiceMock.Setup(service => service.CompleteUserRegistrationAsync(completeSignUpDto))
                .ReturnsAsync(IdentityResult.Success);

            _userServiceMock.Setup(service => service.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync((string)null); // Simulate token generation failure

            // Act
            var result = await _controller.CompleteSignUp(completeSignUpDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Failed to generate email confirmation token.", apiResponse.Message);
        }

        [Fact]
        public async Task CompleteSignUp_ShouldReturnInternalServerError_WhenEmailSendFails()
        {
            // Arrange
            var completeSignUpDto = new CompleteSignUpDto { Email = "testuser@example.com", Username = "testuser", ExternalSignIn = false };

            _userServiceMock.Setup(service => service.CompleteUserRegistrationAsync(completeSignUpDto))
                .ReturnsAsync(IdentityResult.Success);

            _userServiceMock.Setup(service => service.GenerateEmailConfirmationTokenAsync(It.IsAny<string>()))
                .ReturnsAsync("email-token");

            _mailServiceMock.Setup(service => service.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false); // Simulate email send failure

            // Mock the IUrlHelper
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(h => h.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/confirm-email"); // Simulate a valid URL for the confirmation link

            // Set the mocked IUrlHelper in the controller
            _controller.Url = urlHelperMock.Object;

            // Act
            var result = await _controller.CompleteSignUp(completeSignUpDto);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<object>>(internalServerErrorResult.Value);
            Assert.Equal("Error sending confirmation email.", apiResponse.Message);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldRedirect_WhenEmailConfirmationIsSuccessful()
        {
            // Arrange
            var token = "email-token";
            var email = "testuser@example.com";
            var user = new User() { Email = email };

            _userServiceMock.Setup(service => service.ConfirmEmailAsync(email, token))
                .ReturnsAsync(IdentityResult.Success);
            _userServiceMock.Setup(service => service.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _userServiceMock.Setup(service => service.GenerateJwtToken(user))
                .ReturnsAsync("jwt-token");

            // Act
            var result = await _controller.ConfirmEmail(token, email);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Contains("confirm-email?token=jwt-token", redirectResult.Url);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnBadRequest_WhenTokenOrEmailIsInvalid()
        {
            // Act
            var result = await _controller.ConfirmEmail(null, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("Invalid email or token.", apiResponse.Message);
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnBadRequest_WhenUserNotFound()
        {
            // Arrange
            var token = "email-token";
            var email = "testuser@example.com";

            _userServiceMock.Setup(service => service.ConfirmEmailAsync(email, token))
                .ReturnsAsync(IdentityResult.Success); // Email confirmation succeeds

            _userServiceMock.Setup(service => service.FindByEmailAsync(email))
                .ReturnsAsync((User)null); // Simulate user not found after confirmation

            // Act
            var result = await _controller.ConfirmEmail(token, email);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(badRequestResult.Value);
            Assert.Equal("User not found.", apiResponse.Message);
        }

        [Fact]
        public async Task ExternalSignIn_ShouldReturnOk_WhenExternalSignInIsSuccessful()
        {
            // Arrange
            var externalSignInDto = new ExternalSignInDto { Credential = "google-token" };
            var payload = new GoogleJsonWebSignature.Payload() { Email = "testuser@example.com", GivenName = "Test", FamilyName = "User" };

            _googleAuthHelperMock.Setup(helper => helper.VerifyGoogleTokenAsync(externalSignInDto.Credential))
                .ReturnsAsync(payload);
            var user = new User() { Email = payload.Email };
            _userServiceMock.Setup(service => service.FindByEmailAsync(payload.Email))
                .ReturnsAsync(user);
            _userServiceMock.Setup(service => service.GenerateJwtToken(user))
                .ReturnsAsync("jwt-token");

            // Act
            var result = await _controller.ExternalSignIn(externalSignInDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("jwt-token", apiResponse.Data.GetType().GetProperty("Token").GetValue(apiResponse.Data));
        }

        [Fact]
        public async Task ExternalSignIn_ShouldReturnUnauthorized_WhenGoogleTokenIsInvalid()
        {
            // Arrange
            var externalSignInDto = new ExternalSignInDto { Credential = "invalid-google-token" };

            _googleAuthHelperMock.Setup(helper => helper.VerifyGoogleTokenAsync(externalSignInDto.Credential))
                .ReturnsAsync((GoogleJsonWebSignature.Payload)null); // Simulate invalid token

            // Act
            var result = await _controller.ExternalSignIn(externalSignInDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("Invalid token", apiResponse.Message);
        }

        [Fact]
        public async Task ExternalSignIn_ShouldReturnOk_WhenNewUserIsRegistered()
        {
            // Arrange
            var externalSignInDto = new ExternalSignInDto { Credential = "google-token" };
            var payload = new GoogleJsonWebSignature.Payload() { Email = "newuser@example.com", GivenName = "New", FamilyName = "User" };

            _googleAuthHelperMock.Setup(helper => helper.VerifyGoogleTokenAsync(externalSignInDto.Credential))
                .ReturnsAsync(payload); // Google token is valid

            _userServiceMock.Setup(service => service.FindByEmailAsync(payload.Email))
                .ReturnsAsync((User)null); // Simulate user not found, new user registration

            _userServiceMock.Setup(service => service.RegisterExternalUserAsync(It.IsAny<ExternalUserDto>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ExternalSignIn(externalSignInDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);

            // Access the properties of the Data object directly
            var data = apiResponse.Data;
            Assert.NotNull(data); // Ensure the Data is not null

            // Use reflection to get the 'SignUp' property and check its value
            var signUpValue = data.GetType().GetProperty("SignUp")?.GetValue(data);
            Assert.NotNull(signUpValue); // Ensure SignUp is present
            Assert.True((bool)signUpValue); // Ensure SignUp is true
        }

        [Fact]
        public async Task SignIn_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var signInDto = new SignInDto { Email = "testuser@example.com", Password = "Password123!" };
            _userServiceMock.Setup(service => service.LoginUserAsync(signInDto))
                .ReturnsAsync("jwt-token");

            // Act
            var result = await _controller.SignIn(signInDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.Equal("jwt-token", apiResponse.Data.GetType().GetProperty("Token").GetValue(apiResponse.Data));
        }

        [Fact]
        public async Task SignIn_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var signInDto = new SignInDto { Email = "testuser@example.com", Password = "InvalidPassword!" };
            _userServiceMock.Setup(service => service.LoginUserAsync(signInDto))
                .ReturnsAsync((string)null); // Invalid credentials

            // Act
            var result = await _controller.SignIn(signInDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<object>>(unauthorizedResult.Value);
            Assert.Equal("Invalid credentials", apiResponse.Message);
        }
    }
}
