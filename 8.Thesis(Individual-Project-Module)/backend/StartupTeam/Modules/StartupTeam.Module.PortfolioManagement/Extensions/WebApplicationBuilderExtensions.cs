using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.PortfolioManagement.Data;
using StartupTeam.Module.PortfolioManagement.Services;

namespace StartupTeam.Module.PortfolioManagement.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IServiceCollection AddPortfolioManagement(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PortfolioManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IPortfolioService, PortfolioService>();

            return services;
        }
    }
}
