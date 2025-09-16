using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.TeamManagement.Models;

namespace StartupTeam.Module.TeamManagement.Data
{
    public class TeamManagementDbContext : DbContext
    {
        public TeamManagementDbContext(DbContextOptions<TeamManagementDbContext> options)
            : base(options)
        {

        }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<TeamRole> TeamRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure schema for TeamManagement tables
            modelBuilder.Entity<Goal>(entity => entity.ToTable("Goals", "TeamManagement"));
            modelBuilder.Entity<Milestone>(entity => entity.ToTable("Milestones", "TeamManagement"));
            modelBuilder.Entity<Team>(entity => entity.ToTable("Teams", "TeamManagement"));
            modelBuilder.Entity<TeamMember>(entity => entity.ToTable("TeamMembers", "TeamManagement"));
            modelBuilder.Entity<TeamRole>(entity => entity.ToTable("TeamRoles", "TeamManagement"));

            // Goal entity configurations
            modelBuilder
                .Entity<Goal>()
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<Goal>()
                .Property(x => x.Description)
                .IsRequired();

            // Milestone entity configurations
            modelBuilder
                .Entity<Milestone>()
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<Milestone>()
                .Property(x => x.Description)
                .IsRequired();

            // Team entity configurations
            modelBuilder
                .Entity<Team>()
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<Team>()
                .Property(x => x.Description)
                .IsRequired();

            //TeamMember entity configurations
            modelBuilder
                .Entity<TeamMember>()
                .HasOne(tm => tm.TeamRole)
                .WithMany(tr => tr.TeamMembers)
                .HasForeignKey(tm => tm.TeamRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // TeamRole entity configurations
            modelBuilder
                .Entity<TeamRole>()
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<TeamRole>()
                .Property(x => x.Description)
                .IsRequired();
        }
    }
}
