using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.MatchingManagement.Services;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.MatchingManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet("founder/matched-applicants")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTopMatchedApplicants([FromQuery] Guid jobAdId)
        {
            int topN = 10;

            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            var matchedApplicants =
                await _matchService.GetTopMatchedApplicantsForJob(jobAdId, topN);

            if (!matchedApplicants.Any())
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "No matched applicants found."
                });
            }

            return Ok(new ApiResponse<object>()
            {
                Data = matchedApplicants
            });
        }

        [HttpGet("individuals/matched-job-ads")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> GetTopMatchedJobAdsForIndividual()
        {
            int topN = 10;

            var userId = ClaimsHelper.GetUserId(User);

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            var matchedJobAds =
                await _matchService.GetTopMatchedJobAdsForIndividual(userId.Value, topN);

            if (!matchedJobAds.Any())
            {
                return NotFound(new ApiResponse<object>()
                {
                    Success = false,
                    Message = "No matched job advertisements found."
                });
            }

            return Ok(new ApiResponse<object>()
            {
                Data = matchedJobAds
            });
        }
    }
}
