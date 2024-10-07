using SatelliteService.Data.Abstract;
using SatelliteService.Models;

namespace SatelliteService.Data;

public class SatelliteRepository(AppDbContext context) : ISatelliteRepository
{
    public bool SaveChanges() => (context.SaveChanges() >= 0);

    public IEnumerable<Satellite> GetAll() => context.Satellites.ToList();

    public Satellite? GetById(int id) => context.Satellites.FirstOrDefault(s => s.Id == id);

    public void Create(Satellite satellite)
    {
        ArgumentNullException.ThrowIfNull(satellite);

        context.Satellites.Add(satellite);
    }
}