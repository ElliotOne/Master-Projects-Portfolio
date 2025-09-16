using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StartupTeam.Module.TeamManagement.Services
{
    public interface ITeamAuthorizationService
    {
        Task<IActionResult> AuthorizeTeamAction(Guid teamId, ClaimsPrincipal user);
    }
}
