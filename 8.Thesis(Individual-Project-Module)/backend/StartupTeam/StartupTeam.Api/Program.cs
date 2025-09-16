using StartupTeam.Module.JobManagement.Extensions;
using StartupTeam.Module.MatchingManagement.Extensions;
using StartupTeam.Module.PortfolioManagement.Extensions;
using StartupTeam.Module.TeamManagement.Extensions;
using StartupTeam.Module.UserManagement.Data;
using StartupTeam.Module.UserManagement.Extensions;
using StartupTeam.Shared.Extensions;
using System.Text.Json.Serialization;

namespace StartupTeam.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            // Register services from all modules
            builder.Services.AddJobManagement(configuration);
            builder.Services.AddMatchingManagement();
            builder.Services.AddPortfolioManagement(configuration);
            builder.Services.AddTeamManagement(configuration);
            builder.Services.AddUserManagement(configuration);

            // Register shared services
            builder.Services.AddBlobStorageService(configuration);
            builder.Services.AddMailService();

            //Add cross-origin resource sharing support
            var allowedOrigins =
                builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

            if (allowedOrigins != null)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
                });
            }
            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            // Add services to the container.
            builder.Services.AddControllers()
                .AddJsonOptions(jo =>
                {
                    jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            // Ensure Database is populated
            using (var scope = app.Services.CreateScope())
            {
                var dbSeeder = scope.ServiceProvider.GetRequiredService<UserManagementDbSeeder>();
                dbSeeder.SeedDatabaseAsync().Wait();
            }

            app.Run();
        }
    }
}