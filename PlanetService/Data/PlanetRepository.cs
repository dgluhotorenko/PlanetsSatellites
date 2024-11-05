using PlanetService.Data.Abstract;
using PlanetService.Models;

namespace PlanetService.Data;

public class PlanetRepository(AppDbContext context) : IPlanetRepository
{
    public bool SaveChanges() => (context.SaveChanges() >= 0);

    public IEnumerable<Planet> GetAll() => context.Planets.ToList();

    public Planet? GetById(int id) => context.Planets.FirstOrDefault(s => s.Id == id);

    public void Create(Planet planet)
    {
        ArgumentNullException.ThrowIfNull(planet);

        context.Planets.Add(planet);
    }
}