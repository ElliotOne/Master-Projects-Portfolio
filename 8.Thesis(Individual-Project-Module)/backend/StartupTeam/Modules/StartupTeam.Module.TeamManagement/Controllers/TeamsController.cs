using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.TeamManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamAuthorizationService _teamAuthorizationService;

        public TeamsController(
            ITeamService teamService,
            ITeamAuthorizationService teamAuthorizationService)
        {
            _teamService = teamService;
            _teamAuthorizationService = teamAuthorizationService;
        }

        [HttpGet("individual-teams")]
        [Authorize(Roles = RoleConstants.SkilledIndividual)]
        public async Task<IActionResult> GetIndividualTeams()
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

            var teams =
                await _teamService.GetTeamsByIndividualIdAsync(userId.Value);

            return Ok(new ApiResponse<object>()
            {
                Data = teams
            });
        }

        [HttpGet("founder-teams")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetFounderTeams()
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

            var teams =
                await _teamService.GetTeamsByFounderIdAsync(userId.Value);

            return Ok(new ApiResponse<object>
            {
                Data = teams
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTeamForm(Guid id)
        {
            var team = await _teamService.GetTeamFormByIdAsync(id);

            if (team == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team not found."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(team.Id!.Value, User);
            if (authResult is not OkResult) return authResult;

            return Ok(new ApiResponse<TeamFormDto>
            {
                Data = team
            });
        }

        [HttpGet("details/{id}")]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetTeamDetail(Guid id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);

            if (team == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team not found."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(team.Id, User);
            if (authResult is not OkResult) return authResult;

            return Ok(new ApiResponse<TeamDetailDto>
            {
                Data = team
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> CreateTeam(TeamFormDto teamFormDto)
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

            teamFormDto.UserId = userId.Value;

            var result =
                await _teamService.CreateTeamAsync(teamFormDto);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while creating the team."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team created successfully."
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateTeam(Guid id, TeamFormDto teamFormDto)
        {
            if (id != teamFormDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(id, User);
            if (authResult is not OkResult) return authResult;

            var result = await _teamService.UpdateTeamAsync(teamFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(id, User);
            if (authResult is not OkResult) return authResult;

            var result = await _teamService.DeleteTeamAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Team deleted successfully."
            });
        }
    }
}
