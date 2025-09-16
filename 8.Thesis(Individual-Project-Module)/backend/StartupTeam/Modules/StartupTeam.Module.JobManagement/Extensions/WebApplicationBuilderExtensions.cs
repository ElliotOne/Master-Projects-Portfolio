using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.JobManagement.Data;
using StartupTeam.Module.JobManagement.Services;

namespace StartupTeam.Module.JobManagement.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IServiceCollection AddJobManagement(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<JobManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IJobService, JobService>();

            return services;
        }
    }
}
