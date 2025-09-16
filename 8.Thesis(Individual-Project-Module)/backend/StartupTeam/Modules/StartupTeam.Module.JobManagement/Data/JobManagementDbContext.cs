using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.JobManagement.Models;

namespace StartupTeam.Module.JobManagement.Data
{
    public class JobManagementDbContext : DbContext
    {
        public JobManagementDbContext(DbContextOptions<JobManagementDbContext> options)
        : base(options)
        {
        }

        public DbSet<JobAdvertisement> JobAdvertisements { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure schema for JobManagement tables
            modelBuilder.Entity<JobAdvertisement>(entity => entity.ToTable("JobAdvertisements", "JobManagement"));
            modelBuilder.Entity<JobApplication>(entity => entity.ToTable("JobApplications", "JobManagement"));

            // JobAdvertisement entity configurations
            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.StartupName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.StartupDescription)
                .IsRequired();

            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.Industry)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.StartupWebsite)
                .HasMaxLength(256);

            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.JobTitle)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.JobDescription)
                .IsRequired();

            modelBuilder
                .Entity<JobAdvertisement>()
                .Property(x => x.SalaryRange)
                .HasMaxLength(256);

            // JobApplication entity configurations
            modelBuilder
                .Entity<JobApplication>()
                .Property(x => x.CVUrl)
                .HasMaxLength(256);

            modelBuilder
                .Entity<JobApplication>()
                .Property(x => x.CoverLetterUrl)
                .HasMaxLength(256);
        }
    }
}
