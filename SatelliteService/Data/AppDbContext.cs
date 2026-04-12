using Microsoft.EntityFrameworkCore;
using SatelliteService.Models;

namespace SatelliteService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Planet> Planets { get; init; }

    public DbSet<Satellite> Satellites { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Planet>()
            .HasMany(p => p.Satellites)
            .WithOne(s => s.Planet!)
            .HasForeignKey(s => s.PlanetId);
    }
}
