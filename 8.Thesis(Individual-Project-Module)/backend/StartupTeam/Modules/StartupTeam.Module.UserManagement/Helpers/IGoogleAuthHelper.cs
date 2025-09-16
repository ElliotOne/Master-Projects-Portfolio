using Google.Apis.Auth;

namespace StartupTeam.Module.UserManagement.Helpers
{
    public interface IGoogleAuthHelper
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string token);
    }
}
