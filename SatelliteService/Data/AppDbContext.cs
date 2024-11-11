using Microsoft.EntityFrameworkCore;
using SatelliteService.Models;

namespace SatelliteService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Planet> Planets { get; set; }

    public DbSet<Satellite> Satellites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Planet>()
            .HasMany(p => p.Satellites)
            .WithOne(s => s.Planet!)
            .HasForeignKey(s => s.PlanetId);

        modelBuilder
            .Entity<Satellite>()
            .HasOne(s => s.Planet)
            .WithMany(p => p.Satellites)
            .HasForeignKey(s => s.PlanetId);
    }
}