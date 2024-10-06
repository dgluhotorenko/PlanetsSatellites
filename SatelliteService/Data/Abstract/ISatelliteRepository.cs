using SatelliteService.Models;

namespace SatelliteService.Data.Abstract;

public interface ISatelliteRepository
{
    bool SaveChanges();

    IEnumerable<Satellite> GetAll();

    Satellite? GetById(int id);

    void Create(Satellite satellite);
}