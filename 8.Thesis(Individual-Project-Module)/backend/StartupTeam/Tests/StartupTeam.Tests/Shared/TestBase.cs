using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StartupTeam.Tests.Shared
{
    public abstract class TestBase
    {
        protected void MockUser(ControllerBase controller, Guid? userId = null, bool isAuthenticated = true)
        {
            var claims = new List<Claim>();

            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            }

            var identity = new ClaimsIdentity(claims, isAuthenticated ? "mock" : null); // Add authentication type if authenticated
            var user = new ClaimsPrincipal(identity);

            // Set the user in the controller's HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
