using Microsoft.EntityFrameworkCore;
using SatelliteService.Data;
using SatelliteService.Models;

namespace SatelliteService.Tests;

public class SatelliteRepositoryTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void GetAllPlanets_ReturnsPlanets()
    {
        using var context = CreateDbContext();
        context.Planets.Add(new Planet { ExternalId = 1, Name = "Earth" });
        context.SaveChanges();
        var repo = new SatelliteRepository(context);

        var result = repo.GetAllPlanets().ToList();

        Assert.Single(result);
        Assert.Equal("Earth", result[0].Name);
    }

    [Fact]
    public void IsPlanetExists_ExistingPlanet_ReturnsTrue()
    {
        using var context = CreateDbContext();
        var planet = new Planet { ExternalId = 1, Name = "Earth" };
        context.Planets.Add(planet);
        context.SaveChanges();
        var repo = new SatelliteRepository(context);

        Assert.True(repo.IsPlanetExists(planet.Id));
    }

    [Fact]
    public void IsPlanetExists_NonExisting_ReturnsFalse()
    {
        using var context = CreateDbContext();
        var repo = new SatelliteRepository(context);

        Assert.False(repo.IsPlanetExists(999));
    }

    [Fact]
    public void IsExternalPlanetExists_ExistingExternalId_ReturnsTrue()
    {
        using var context = CreateDbContext();
        context.Planets.Add(new Planet { ExternalId = 42, Name = "Mars" });
        context.SaveChanges();
        var repo = new SatelliteRepository(context);

        Assert.True(repo.IsExternalPlanetExists(42));
    }

    [Fact]
    public void CreatePlanet_AddsPlanet()
    {
        using var context = CreateDbContext();
        var repo = new SatelliteRepository(context);

        repo.CreatePlanet(new Planet { ExternalId = 1, Name = "Venus" });
        repo.SaveChanges();

        Assert.Single(context.Planets);
    }

    [Fact]
    public void GetSatellitesByPlanetId_ReturnsSortedByName()
    {
        using var context = CreateDbContext();
        var planet = new Planet { ExternalId = 1, Name = "Earth" };
        context.Planets.Add(planet);
        context.SaveChanges();

        context.Satellites.AddRange(
            new Satellite { Name = "Moon", Type = "Natural", PlanetId = planet.Id },
            new Satellite { Name = "ISS", Type = "Artificial", PlanetId = planet.Id }
        );
        context.SaveChanges();
        var repo = new SatelliteRepository(context);

        var result = repo.GetSatellitesByPlanetId(planet.Id).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("ISS", result[0].Name);
        Assert.Equal("Moon", result[1].Name);
    }

    [Fact]
    public void GetSatellite_ExistingSatellite_ReturnsSatellite()
    {
        using var context = CreateDbContext();
        var planet = new Planet { ExternalId = 1, Name = "Earth" };
        context.Planets.Add(planet);
        context.SaveChanges();

        var satellite = new Satellite { Name = "Moon", Type = "Natural", PlanetId = planet.Id };
        context.Satellites.Add(satellite);
        context.SaveChanges();
        var repo = new SatelliteRepository(context);

        var result = repo.GetSatellite(planet.Id, satellite.Id);

        Assert.NotNull(result);
        Assert.Equal("Moon", result.Name);
    }

    [Fact]
    public void GetSatellite_NonExisting_ReturnsNull()
    {
        using var context = CreateDbContext();
        var repo = new SatelliteRepository(context);

        var result = repo.GetSatellite(1, 999);

        Assert.Null(result);
    }

    [Fact]
    public void CreateSatellite_SetsPlanetIdAndAdds()
    {
        using var context = CreateDbContext();
        var planet = new Planet { ExternalId = 1, Name = "Earth" };
        context.Planets.Add(planet);
        context.SaveChanges();
        var repo = new SatelliteRepository(context);

        var satellite = new Satellite { Name = "Moon", Type = "Natural" };
        repo.CreateSatellite(planet.Id, satellite);
        repo.SaveChanges();

        Assert.Single(context.Satellites);
        Assert.Equal(planet.Id, context.Satellites.First().PlanetId);
    }
}
