using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.TeamManagement.Dtos;
using StartupTeam.Module.TeamManagement.Services;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;

namespace StartupTeam.Module.TeamManagement.Controllers
{
    [ApiController]
    [Route("api/teams/{teamId}/goals")]
    public class GoalsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ITeamAuthorizationService _teamAuthorizationService;

        public GoalsController(
            ITeamService teamService,
            ITeamAuthorizationService teamAuthorizationService)
        {
            _teamService = teamService;
            _teamAuthorizationService = teamAuthorizationService;
        }

        [HttpGet]
        [Authorize(Roles = RoleConstants.SkilledIndividual + "," + RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetGoals(Guid teamId)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var goals =
                await _teamService.GetGoalsByTeamIdAsync(teamId);

            return Ok(new ApiResponse<object>()
            {
                Data = goals
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> GetGoalForm(Guid id)
        {
            var goal = await _teamService.GetGoalFormByIdAsync(id);

            if (goal == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Goal not found."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(goal.TeamId, User);
            if (authResult is not OkResult) return authResult;

            return Ok(new ApiResponse<GoalFormDto>
            {
                Data = goal
            });
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> CreateGoal(GoalFormDto goalFormDto)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(goalFormDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            var result =
                await _teamService.CreateGoalAsync(goalFormDto);

            if (!result)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "An error occurred while creating the goal."
                    });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Goal created successfully."
            });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> UpdateGoal(Guid id, GoalFormDto goalFormDto)
        {
            if (id != goalFormDto.Id)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID mismatch."
                });
            }

            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(goalFormDto.TeamId, User);
            if (authResult is not OkResult) return authResult;

            var result =
                await _teamService.UpdateGoalAsync(goalFormDto);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Goal not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Goal updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.StartupFounder)]
        public async Task<IActionResult> DeleteGoal(Guid teamId, Guid id)
        {
            var authResult = await _teamAuthorizationService.AuthorizeTeamAction(teamId, User);
            if (authResult is not OkResult) return authResult;

            var result = await _teamService.DeleteGoalAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Goal not found."
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Goal deleted successfully."
            });
        }
    }
}
