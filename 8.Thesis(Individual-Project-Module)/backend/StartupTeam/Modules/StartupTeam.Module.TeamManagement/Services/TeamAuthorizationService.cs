using Microsoft.AspNetCore.Mvc;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models.Constants;
using StartupTeam.Shared.Models;
using System.Security.Claims;

namespace StartupTeam.Module.TeamManagement.Services
{
    public class TeamAuthorizationService : ITeamAuthorizationService
    {
        private readonly ITeamService _teamService;

        public TeamAuthorizationService(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<IActionResult> AuthorizeTeamAction(Guid teamId, ClaimsPrincipal user)
        {
            var userId = ClaimsHelper.GetUserId(user);

            if (userId == null)
            {
                return new BadRequestObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User ID is missing or invalid."
                });
            }

            var team = await _teamService.GetTeamByIdAsync(teamId);

            if (team == null)
            {
                return new NotFoundObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Team not found."
                });
            }

            if ((team.UserId != userId && user.IsInRole(RoleConstants.StartupFounder)) ||
                (!await _teamService.IsIndividualMemberOfTeam(teamId, userId.Value)
                 && user.IsInRole(RoleConstants.SkilledIndividual)))
            {
                return new UnauthorizedObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "You are not authorized to perform this action."
                });
            }

            return new OkResult();
        }
    }
}
