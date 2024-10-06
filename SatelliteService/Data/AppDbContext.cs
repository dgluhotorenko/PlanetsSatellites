using Microsoft.EntityFrameworkCore;
using SatelliteService.Models;

namespace SatelliteService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Satellite> Satellites { get; init; }
}