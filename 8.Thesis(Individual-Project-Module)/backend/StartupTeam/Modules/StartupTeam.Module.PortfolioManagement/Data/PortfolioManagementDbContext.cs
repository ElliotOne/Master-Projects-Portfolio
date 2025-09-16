using Microsoft.EntityFrameworkCore;
using StartupTeam.Module.PortfolioManagement.Models;

namespace StartupTeam.Module.PortfolioManagement.Data
{
    public class PortfolioManagementDbContext : DbContext
    {
        public PortfolioManagementDbContext(DbContextOptions<PortfolioManagementDbContext> options)
            : base(options)
        {

        }

        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<PortfolioItem> PortfolioItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure schema for PortfolioManagement tables
            modelBuilder.Entity<Portfolio>(entity => entity.ToTable("Portfolios", "PortfolioManagement"));
            modelBuilder.Entity<PortfolioItem>(entity => entity.ToTable("PortfolioItems", "PortfolioManagement"));

            //PortfolioManagement entity configurations
            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Description)
                .IsRequired();

            modelBuilder.Entity<PortfolioItem>()
                .Property(x => x.Type)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Technologies)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Skills)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Industry)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Role)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Duration)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Link)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.AttachmentUrl)
                .HasMaxLength(256);

            modelBuilder
                .Entity<PortfolioItem>()
                .Property(x => x.Tags)
                .HasMaxLength(256);
        }
    }
}
