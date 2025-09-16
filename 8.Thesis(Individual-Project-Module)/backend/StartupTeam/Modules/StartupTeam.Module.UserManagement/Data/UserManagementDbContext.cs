using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.UserManagement.Models;

namespace StartupTeam.Module.UserManagement.Data
{
    public class UserManagementDbContext : IdentityDbContext<User, Role, Guid>
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure schema for Identity tables
            modelBuilder.Entity<User>(entity => entity.ToTable("Users", "UserManagement"));
            modelBuilder.Entity<Role>(entity => entity.ToTable("Roles", "UserManagement"));
            modelBuilder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UserRoles", "UserManagement"));
            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UserClaims", "UserManagement"));
            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UserLogins", "UserManagement"));
            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RoleClaims", "UserManagement"));
            modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens", "UserManagement"));

            // User entity configurations
            modelBuilder
                .Entity<User>()
                .Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(x => x.ProfilePictureUrl)
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<User>()
                .Property(x => x.PhoneNumber)
                .HasMaxLength(256);
        }
    }
}
