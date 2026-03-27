using Microsoft.EntityFrameworkCore;
using TelemetryService.Models;

namespace TelemetryService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Telemetry> Telemetry { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Telemetry>()
            .HasIndex(t => new { t.DeviceId, t.Timestamp });

         modelBuilder.HasPostgresExtension("timescaledb");
        modelBuilder.Entity<Telemetry>().ToTable(tb => tb.HasComment("HYPER TABLE"));
    }
}