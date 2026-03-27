namespace GateService.Data
{
    using global::GateService.Models;
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Gate> Gates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gate>()
                .Property(g => g.Status)
                .HasDefaultValue("closed");

            modelBuilder.Entity<Gate>()
                .Property(g => g.LastUpdated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Gate>()
                .Property(g => g.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
