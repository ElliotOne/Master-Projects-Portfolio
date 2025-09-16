using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StartupTeam.Shared.Models;
using StartupTeam.Shared.Services;

namespace StartupTeam.Shared.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static void AddBlobStorageService(
            this IServiceCollection services, IConfiguration configuration)
        {
            var blobStorageConnectionString = configuration["BlobStorage:ConnectionString"]!;

            services.AddSingleton<IBlobStorageService>(new BlobStorageService(blobStorageConnectionString));
        }

        public static void AddMailService(this IServiceCollection services)
        {
            services.AddOptions<EmailSettings>()
                .BindConfiguration(nameof(EmailSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddTransient<IMailService, MailService>();
        }
    }
}
