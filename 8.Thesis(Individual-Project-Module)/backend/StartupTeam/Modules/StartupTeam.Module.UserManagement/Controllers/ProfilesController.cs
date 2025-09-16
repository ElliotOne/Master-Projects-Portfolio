using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.UserManagement.Dtos;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Module.UserManagement.Services;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.UserManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
    public class ProfilesController : Controller
    {
        private readonly IUserService _userService;

        public ProfilesController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all-profiles")]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _userService.GetProfilesAsync();

            return Ok(new ApiResponse<object>()
            {
                Data = profiles
            });
        }

        [HttpGet("details/{username}")]
        public async Task<IActionResult> GetProfileDetail(string username)
        {
            var profile = await _userService.GetProfileByUsernameAsync(username);

            if (profile == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Profile not found."
                });
            }

            return Ok(new ApiResponse<ProfileDetailDto>
            {
                Data = profile
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileForm()
        {
            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            var profile = await _userService.GetProfileFormByUserIdAsync(userId.Value);

            if (profile == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Profile not found."
                });
            }

            return Ok(new ApiResponse<object>()
            {
                Data = profile
            });
        }

        [HttpPut("{userName}")]
        public async Task<IActionResult> UpdateProfile(
            string userName, ProfileFormDto profileFormDto)
        {
            if (userName != profileFormDto.UserName)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "UserName mismatch."
                });
            }

            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            if (userId != profileFormDto.UserId)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to update this profile."
                });
            }

            var result = await _userService.UpdateProfileAsync(profileFormDto);

            if (!result.Succeeded)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Profile not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Profile updated successfully."
            });
        }
    }
}
