using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.TeamManagement.Data;
using StartupTeam.Module.TeamManagement.Services;

namespace StartupTeam.Module.TeamManagement.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IServiceCollection AddTeamManagement(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TeamManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ITeamAuthorizationService, TeamAuthorizationService>();

            return services;
        }
    }
}
