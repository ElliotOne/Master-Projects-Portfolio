using System.Security.Claims;

namespace StartupTeam.Module.UserManagement.Helpers
{
    public static class ClaimsHelper
    {
        public static Guid? GetUserId(ClaimsPrincipal user)
        {
            // Ensure user is authenticated
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdClaim?.Value, out var userId))
                {
                    return userId;
                }
            }
            return null;
        }
    }
}
