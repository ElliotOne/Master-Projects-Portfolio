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
    public class JobApplicationsController : Controller
    {
        private readonly IJobService _jobService;

        public JobApplicationsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("individual-applications")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> GetIndividualJobApplications()
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

            var jobApplications =
                await _jobService.GetJobApplicationsByIndividualIdAsync(userId.Value);

            return Ok(new ApiResponse<object>()
            {
                Data = jobApplications
            });
        }

        [HttpGet("founder-applications")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetFounderJobApplications([FromQuery] Guid? jobId = null)
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

            var jobApplications =
                await _jobService.GetJobApplicationsByFounderIdAsync(userId.Value, jobId);

            return Ok(new ApiResponse<object>()
            {
                Data = jobApplications
            });
        }

        [HttpGet("{id}/successful-applicants")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetSuccessfulJobApplicants(Guid id)
        {
            var jobApplicantDtos =
                await _jobService.GetSuccessfulJobApplicantsByJobAdvertisementIdAsync(id);

            return Ok(new ApiResponse<object>()
            {
                Data = jobApplicantDtos
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> SubmitIndividualJobApplication(
            JobApplicationFormDto applicationFormDto)
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
                await _jobService.GetJobAdvertisementByIdAsync(
                    applicationFormDto.JobAdvertisementId, userId.Value);

            if (jobAdvertisement == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Job advertisement not found."
                });
            }

            if (await _jobService.HasUserAlreadyAppliedAsync(jobAdvertisement.Id, userId.Value))
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "You have already applied for this job."
                });
            }

            // Validate if CV is required
            if (jobAdvertisement.RequireCV && applicationFormDto.CVFile == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "CV is required for this job application."
                });
            }

            // Validate if Cover letter is required
            if (jobAdvertisement.RequireCoverLetter && applicationFormDto.CoverLetterFile == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Cover letter is required for this job application."
                });
            }

            var result =
                await _jobService.SubmitJobApplicationAsync(applicationFormDto, userId.Value);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while submitting the job application."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Job application submitted successfully."
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetJobApplicationDetails(Guid id)
        {
            var jobApplication = await _jobService.GetJobApplicationFormByIdAsync(id);

            if (jobApplication == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Job application not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Data = jobApplication
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateJobApplication(
            Guid id, JobApplicationUpdateFormDto jobApplicationUpdateFormDto)
        {
            if (id != jobApplicationUpdateFormDto.Id)
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

            var result =
                await _jobService.UpdateJobApplicationAsync(jobApplicationUpdateFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Job application not found or you are not authorized to update it."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Job application status updated successfully."
            });
        }

        [HttpPut("{id}/update-status")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> UpdateApplicationStatus(
            Guid id, JobApplicationUpdateFormDto jobApplicationUpdateFormDto)
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

            var jobApplication = await _jobService.GetJobApplicationByIdAsync(id);

            if (jobApplication == null)
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "Job application not found."
                });
            }

            if (userId != jobApplication.UserId)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to update this job application."
                });
            }

            var result =
                await _jobService.UpdateApplicationStatusByIndividualAsync(id, jobApplicationUpdateFormDto.Status);

            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Unable to update the application status."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Application status updated successfully."
            });
        }
    }
}
