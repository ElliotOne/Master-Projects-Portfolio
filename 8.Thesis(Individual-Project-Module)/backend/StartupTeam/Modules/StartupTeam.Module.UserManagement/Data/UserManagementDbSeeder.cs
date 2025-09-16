using Microsoft.AspNetCore.Identity;
using StartupTeam.Module.UserManagement.Models;
using StartupTeam.Module.UserManagement.Models.Constants;

namespace StartupTeam.Module.UserManagement.Data
{
    public class UserManagementDbSeeder
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public UserManagementDbSeeder(
            RoleManager<Role> roleManager,
            UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                RoleConstants.StartupFounder,
                RoleConstants.SkilledIndividual
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new Role() { Name = role });
                }
            }
        }

        public async Task SeedUsersAsync()
        {
            // Create an anonymized Startup Founder user
            var founderEmail = "founder_test@example.com";
            var founderUser = await _userManager.FindByEmailAsync(founderEmail);
            if (founderUser == null)
            {
                founderUser = new User
                {
                    UserName = "founder_user",
                    Email = founderEmail,
                    FirstName = "Startup",
                    LastName = "Creator",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(founderUser, "TestP@ss123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(founderUser, RoleConstants.StartupFounder);
                }
            }

            // Create an anonymized Skilled Individual user
            var individualEmail = "individual_test@example.com";
            var skilledUser = await _userManager.FindByEmailAsync(individualEmail);
            if (skilledUser == null)
            {
                skilledUser = new User
                {
                    UserName = "skilled_user",
                    Email = individualEmail,
                    FirstName = "Talent",
                    LastName = "Member",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(skilledUser, "TestP@ss123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(skilledUser, RoleConstants.SkilledIndividual);
                }
            }
        }

        public async Task SeedDatabaseAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }
    }
}
