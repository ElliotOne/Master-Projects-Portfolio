using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.TeamManagement.Controllers
{
    [ApiController]
    [Route("api/teams/{teamId}/milestones")]
    public class MilestonesController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamAuthorizationService _teamAuthorizationService;

        public MilestonesController(
            ITeamService teamService,
            ITeamAuthorizationService teamAuthorizationService)
        {
            _teamService = teamService;
            _teamAuthorizationService = teamAuthorizationService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetMilestones(Guid teamId)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var milestones =
                await _teamService.GetMilestonesByTeamIdAsync(teamId);

            return Ok(new ApiResponse<object>()
            {
                Data = milestones
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetMilestoneForm(Guid id)
        {
            var milestone = await _teamService.GetMilestoneFormByIdAsync(id);

            if (milestone == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Milestone not found."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(milestone.TeamId, User);
            if (authResult is not OkResult) return authResult;

            return Ok(new ApiResponse<MilestoneFromDto>
            {
                Data = milestone
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> CreateMilestone(MilestoneFromDto milestoneFromDto)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(milestoneFromDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            if (!await _teamService.IsMilestoneDueDateValid(milestoneFromDto))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "The milestone due date must be earlier than the associated goal's deadline."
                });
            }

            var result =
                await _teamService.CreateMilestoneAsync(milestoneFromDto);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while creating the milestone."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Milestone created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateMilestone(Guid id, MilestoneFromDto milestoneFromDto)
        {
            if (id != milestoneFromDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(milestoneFromDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            if (!await _teamService.IsMilestoneDueDateValid(milestoneFromDto))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "The milestone due date must be earlier than the associated goal's deadline."
                });
            }

            var result =
                await _teamService.UpdateMilestoneAsync(milestoneFromDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Milestone not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Milestone updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> DeleteMilestone(Guid teamId, Guid id)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var result = await _teamService.DeleteMilestoneAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Milestone not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Milestone deleted successfully."
            });
        }
    }
}
