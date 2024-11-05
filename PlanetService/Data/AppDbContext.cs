using Microsoft.EntityFrameworkCore;
using PlanetService.Models;

namespace PlanetService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Planet> Planets { get; init; }
}