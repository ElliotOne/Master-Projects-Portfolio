using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Models;
using StartupTeam.Shared.Services;

namespace StartupTeam.Module.UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly IGoogleAuthHelper _googleAuthHelper;
        private readonly string? _confirmEmailRedirectUrl;

        public AuthController(
            IUserService userService,
            IMailService mailService,
            IGoogleAuthHelper googleAuthHelper,
            IConfiguration configuration)
        {
            _userService = userService;
            _mailService = mailService;
            _googleAuthHelper = googleAuthHelper;
            _confirmEmailRedirectUrl = configuration["ConfirmEmailRedirectUrl"];
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(BasicSignUpDto basicSignUpDto)
        {
            var result = await _userService.RegisterUserAsync(basicSignUpDto);

            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>()
                {
                    Data = new { Email = basicSignUpDto.Email }
                });
            }

            return BadRequest(new ApiResponse<object>()
            {
                Success = false,
                Errors = IdentityErrorsToDictionary(result.Errors)
            });
        }

        [HttpPost("complete-signup")]
        public async Task<IActionResult> CompleteSignUp(CompleteSignUpDto completeSignUpDto)
        {
            var result = await _userService.CompleteUserRegistrationAsync(completeSignUpDto);

            if (result.Succeeded)
            {
                if (completeSignUpDto.ExternalSignIn)
                {
                    var user = await _userService.FindByEmailAsync(completeSignUpDto.Email);

                    if (user == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "User not found."
                        });
                    }

                    var jwtToken = await _userService.GenerateJwtToken(user);

                    return Ok(new ApiResponse<object>()
                    {
                        Data = new
                        {
                            ExternalSignIn = true,
                            Token = jwtToken
                        }
                    });
                }

                var token = await _userService.GenerateEmailConfirmationTokenAsync(completeSignUpDto.Username);

                if (token == null)
                {
                    return BadRequest(new ApiResponse<object>()
                    {
                        Success = false,
                        Message = "Failed to generate email confirmation token."
                    });
                }

                var confirmationLink = Url.Action(
                    nameof(ConfirmEmail),
                    ControllerContext.ActionDescriptor.ControllerName,
                    new { token, email = completeSignUpDto.Email },
                    Request.Scheme);

                var subject = "StartupTeam - Confirm your email address";
                var body = $"Please confirm your email by clicking the following link: <a href='{confirmationLink}'>Confirm Email</a>";

                var emailSent = await _mailService.SendEmail(completeSignUpDto.Email, subject, body);

                if (!emailSent)
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse<object>()
                        {
                            Success = false,
                            Message = "Error sending confirmation email."
                        });
                }

                return Ok(new ApiResponse<object>());
            }

            return BadRequest(new ApiResponse<object>()
            {
                Success = false,
                Errors = IdentityErrorsToDictionary(result.Errors)
            });
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Invalid email or token."
                });
            }

            var result = await _userService.ConfirmEmailAsync(email, token);

            if (result.Succeeded)
            {
                var user = await _userService.FindByEmailAsync(email);

                if (user == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                var jwtToken = await _userService.GenerateJwtToken(user);

                var redirectUrl = $"{_confirmEmailRedirectUrl}{jwtToken}";
                return Redirect(redirectUrl);
            }

            return BadRequest(new ApiResponse<object>()
            {
                Success = false,
                Errors = IdentityErrorsToDictionary(result.Errors)
            });
        }

        [HttpPost("external-signin")]
        public async Task<IActionResult> ExternalSignIn(ExternalSignInDto externalSignInDto)
        {
            var payload = await _googleAuthHelper.VerifyGoogleTokenAsync(externalSignInDto.Credential);

            if (payload == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid token"
                });
            }

            var email = payload.Email!;

            var user = await _userService.FindByEmailAsync(email);

            if (user == null)
            {
                var externalUserDto = new ExternalUserDto()
                {
                    Email = email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    ProfilePictureUrl = payload.Picture
                };

                var result = await _userService.RegisterExternalUserAsync(externalUserDto);

                if (result.Succeeded)
                {
                    return Ok(new ApiResponse<object>()
                    {
                        Data = new
                        {
                            SignUp = true,
                            ExternalSignIn = true,
                            Email = externalUserDto.Email,
                            Username = externalUserDto.Email,
                            FirstName = externalUserDto.FirstName,
                            LastName = externalUserDto.LastName
                        }
                    });
                }

                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Errors = IdentityErrorsToDictionary(result.Errors)
                });
            }

            var jwtToken = await _userService.GenerateJwtToken(user);

            return Ok(new ApiResponse<object>()
            {
                Data = new
                {
                    ExternalSignIn = true,
                    Token = jwtToken
                }
            });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInDto signInDto)
        {
            var token = await _userService.LoginUserAsync(signInDto);

            if (token != null)
            {
                return Ok(new ApiResponse<object>()
                {
                    Data = new { Token = token }
                });
            }

            return Unauthorized(new ApiResponse<object>()
            {
                Success = false,
                Message = "Invalid credentials"
            });
        }

        private Dictionary<string, List<string>> IdentityErrorsToDictionary(IEnumerable<IdentityError> errors)
        {
            return errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToList()
                );
        }
    }
}
