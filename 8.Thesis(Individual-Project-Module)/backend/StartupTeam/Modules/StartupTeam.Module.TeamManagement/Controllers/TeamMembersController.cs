using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.TeamManagement.Controllers
{
    [ApiController]
    [Route("api/teams/{teamId}/members")]
    public class TeamMembersController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamAuthorizationService _teamAuthorizationService;

        public TeamMembersController(
            ITeamService teamService,
            ITeamAuthorizationService teamAuthorizationService)
        {
            _teamService = teamService;
            _teamAuthorizationService = teamAuthorizationService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTeamMembers(Guid teamId)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var members =
                await _teamService.GetTeamMembersByTeamIdAsync(teamId);

            return Ok(new ApiResponse<object>
            {
                Data = members
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTeamMemberForm(Guid id)
        {
            var teamMember = await _teamService.GetTeamMemberFormByIdAsync(id);

            if (teamMember == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team member not found."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamMember.TeamId, User);
            if (authResult is not OkResult) return authResult;

            return Ok(new ApiResponse<TeamMemberFormDto>
            {
                Data = teamMember
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> AddTeamMember(TeamMemberFormDto teamMemberFormDto)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamMemberFormDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            // Check if the individual already exists in the team with the same role
            var isIndividualInTeam = await _teamService.IsIndividualInTeamWithRole(
                teamMemberFormDto.TeamId,
                teamMemberFormDto.UserId,
                teamMemberFormDto.TeamRoleId);

            if (isIndividualInTeam)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Individual is already in the team with the same role."
                });
            }

            var result =
                await _teamService.CreateTeamMemberAsync(teamMemberFormDto);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while adding the team member."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team member added successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateTeamMember(Guid id, TeamMemberFormDto teamMemberFormDto)
        {
            if (id != teamMemberFormDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamMemberFormDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            // Check if the individual already exists in the team with the same role
            var isIndividualInTeam = await _teamService.IsIndividualInTeamWithRole(
                teamMemberFormDto.TeamId,
                teamMemberFormDto.UserId,
                teamMemberFormDto.TeamRoleId);

            if (isIndividualInTeam)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Individual is already in the team with the same role."
                });
            }

            var result =
                await _teamService.UpdateTeamMemberAsync(teamMemberFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team member not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team member updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> RemoveTeamMember(Guid teamId, Guid id)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var result =
                await _teamService.DeleteTeamMemberAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team member not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team member removed successfully."
            });
        }
    }
}
