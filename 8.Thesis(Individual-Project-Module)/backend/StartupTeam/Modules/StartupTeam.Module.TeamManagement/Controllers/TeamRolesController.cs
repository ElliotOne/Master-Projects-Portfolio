using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.TeamManagement.Controllers
{
    [ApiController]
    [Route("api/teams/{teamId}/roles")]
    public class TeamRolesController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamAuthorizationService _teamAuthorizationService;

        public TeamRolesController(
            ITeamService teamService,
            ITeamAuthorizationService teamAuthorizationService)
        {
            _teamService = teamService;
            _teamAuthorizationService = teamAuthorizationService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTeamRoles(Guid teamId)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var roles =
                await _teamService.GetTeamRolesByTeamIdAsync(teamId);

            return Ok(new ApiResponse<object>
            {
                Data = roles
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTeamRoleForm(Guid id)
        {
            var role = await _teamService.GetTeamRoleFormByIdAsync(id);

            if (role == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Role not found."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(role.TeamId, User);
            if (authResult is not OkResult) return authResult;

            return Ok(new ApiResponse<TeamRoleFormDto>
            {
                Data = role
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> CreateTeamRole(TeamRoleFormDto teamRoleFormDto)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamRoleFormDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            var result =
                await _teamService.CreateTeamRoleAsync(teamRoleFormDto);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while creating the team role."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team role created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateTeamRole(Guid id, TeamRoleFormDto teamRoleFormDto)
        {
            if (id != teamRoleFormDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamRoleFormDto.TeamId, User);
            if (authResult is not OkResult) return authResult;


            var result =
                await _teamService.UpdateTeamRoleAsync(teamRoleFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team role not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team role updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> DeleteTeamRole(Guid teamId, Guid id)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var result =
                await _teamService.DeleteTeamRoleAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team role not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team role deleted successfully."
            });
        }
    }
}
