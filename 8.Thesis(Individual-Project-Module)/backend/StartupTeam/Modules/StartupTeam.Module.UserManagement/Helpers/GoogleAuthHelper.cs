using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace StartupTeam.Module.UserManagement.Helpers
{
    public class GoogleAuthHelper : IGoogleAuthHelper
    {
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleAuthHelper(IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _googleAuthSettings = googleAuthSettings.Value;
        }

        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleAuthSettings.ClientId }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);
                return payload;
            }
            catch (InvalidJwtException)
            {
                return null;
            }
        }
    }
}
