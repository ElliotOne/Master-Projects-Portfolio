using StartupTeam.Module.UserManagement.Models;

namespace StartupTeam.Module.UserManagement.Helpers
{
    public interface IJwtHelper
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}
