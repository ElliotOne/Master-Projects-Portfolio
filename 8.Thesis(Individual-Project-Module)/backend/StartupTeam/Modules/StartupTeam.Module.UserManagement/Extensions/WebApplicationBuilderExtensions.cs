using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StartupTeam.Module.UserManagement.Data;
using StartupTeam.Module.UserManagement.Helpers;
using StartupTeam.Module.UserManagement.Models;
using StartupTeam.Module.UserManagement.Services;
using System.Security.Claims;
using System.Text;

namespace StartupTeam.Module.UserManagement.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static IServiceCollection AddUserManagement(
            this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            services.AddDbContext<UserManagementDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<UserManagementDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });

            services.AddOptions<GoogleAuthSettings>()
                .BindConfiguration("Authentication:Google")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton(jwtSettings!);
            services.AddScoped<IJwtHelper, JwtHelper>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<UserManagementDbSeeder>();
            services.AddTransient<IGoogleAuthHelper, GoogleAuthHelper>();

            services.AddAuthentication(co =>
                {
                    co.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    co.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    co.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = false,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings!.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings!.SecretKey)),
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,
                    };
                })
            .AddGoogle(options =>
                {
                    options.ClientId = configuration["Authentication:Google:ClientId"]!;
                    options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                });

            return services;
        }
    }
}
