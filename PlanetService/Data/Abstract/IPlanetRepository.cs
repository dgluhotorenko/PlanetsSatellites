using PlanetService.Models;

namespace PlanetService.Data.Abstract;

public interface IPlanetRepository
{
    bool SaveChanges();

    IEnumerable<Planet> GetAll();

    Planet? GetById(int id);

    void Create(Planet planet);
}