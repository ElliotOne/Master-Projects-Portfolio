using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.JobManagement.Dtos;
using StartupTeam.Module.JobManagement.Services;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.JobManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobAdvertisementsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobAdvertisementsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("all-jobs")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetJobAdvertisements()
        {
            var jobAdvertisements =
                await _jobService.GetJobAdvertisementsAsync();

            return Ok(new ApiResponse<object>()
            {
                Data = jobAdvertisements
            });
        }

        [HttpGet("founder-jobs")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetJobAdvertisementsByUserId()
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

            var jobAdvertisements =
                await _jobService.GetJobAdvertisementsByUserIdAsync(userId.Value);

            return Ok(new ApiResponse<object>()
            {
                Data = jobAdvertisements
            });
        }

        [HttpGet("founder-jobs/{userId}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetJobsByUserId(Guid userId)
        {
            var jobAdvertisements =
                await _jobService.GetJobAdvertisementsForUserAsync(userId);

            if (!jobAdvertisements.Any())
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "No job advertisements found for this user."
                });
            }

            return Ok(new ApiResponse<object>()
            {
                Data = jobAdvertisements
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetJobAdvertisementForm(Guid id)
        {
            var jobAdvertisement = await _jobService.GetJobAdvertisementFormByIdAsync(id);

            if (jobAdvertisement == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Job advertisement not found."
                });
            }

            return Ok(new ApiResponse<object>()
            {
                Data = jobAdvertisement
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> CreateJobAdvertisement(
            JobAdvertisementFormDto advertisementFormDto)
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

            var result =
                await _jobService.CreateJobAdvertisementAsync(advertisementFormDto, userId.Value);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while creating the job advertisement."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Job advertisement created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateJobAdvertisement(
            Guid id, JobAdvertisementFormDto advertisementFormDto)
        {
            if (id != advertisementFormDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            if (userId != advertisementFormDto.UserId)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to update this job advertisement."
                });
            }

            var result = await _jobService.UpdateJobAdvertisementAsync(advertisementFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Job advertisement not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Job advertisement updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> DeleteJobAdvertisement(Guid id)
        {
            var result = await _jobService.DeleteJobAdvertisementAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Job advertisement not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Job advertisement deleted successfully."
            });
        }

        [HttpGet("details/{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetJobAdvertisementDetail(Guid id)
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

            var jobAdvertisement =
                await _jobService.GetJobAdvertisementByIdAsync(id, userId.Value);

            if (jobAdvertisement == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Job advertisement not found."
                });
            }

            return Ok(new ApiResponse<JobAdvertisementDetailDto>
            {
                Data = jobAdvertisement
            });
        }
    }
}
