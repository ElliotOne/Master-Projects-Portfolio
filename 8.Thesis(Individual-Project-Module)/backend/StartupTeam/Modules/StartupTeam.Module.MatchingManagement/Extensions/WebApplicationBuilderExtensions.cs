using StartupTeam.Module.MatchingManagement.Services;
using StartupTeam.Module.MatchingManagement.Utilities;

namespace StartupTeam.Module.MatchingManagement.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IServiceCollection AddMatchingManagement(this IServiceCollection services)
        {
            services.AddScoped<TextSimilarityCalculator>();
            services.AddScoped<IMatchService, MatchService>();

            return services;
        }
    }
}
