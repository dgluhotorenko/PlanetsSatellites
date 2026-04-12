using Microsoft.EntityFrameworkCore;
using PlanetService.Data;
using PlanetService.Models;

namespace PlanetService.Tests;

public class PlanetRepositoryTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public void GetAll_ReturnsAllPlanets()
    {
        // Arrange
        using var context = CreateDbContext();
        context.Planets.AddRange(
            new Planet { Name = "Earth", Mass = 1.0, Radius = 6371.0 },
            new Planet { Name = "Mars", Mass = 0.107, Radius = 3389.5 }
        );
        context.SaveChanges();
        var repository = new PlanetRepository(context);

        // Act
        var result = repository.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsPlanet()
    {
        using var context = CreateDbContext();
        var planet = new Planet { Name = "Earth", Mass = 1.0, Radius = 6371.0 };
        context.Planets.Add(planet);
        context.SaveChanges();
        var repository = new PlanetRepository(context);

        var result = repository.GetById(planet.Id);

        Assert.NotNull(result);
        Assert.Equal("Earth", result.Name);
    }

    [Fact]
    public void GetById_NonExistingId_ReturnsNull()
    {
        using var context = CreateDbContext();
        var repository = new PlanetRepository(context);

        var result = repository.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public void Create_AddsPlanetToDatabase()
    {
        using var context = CreateDbContext();
        var repository = new PlanetRepository(context);
        var planet = new Planet { Name = "Venus", Mass = 0.815, Radius = 6051.8 };

        repository.Create(planet);
        repository.SaveChanges();

        Assert.Single(context.Planets);
        Assert.Equal("Venus", context.Planets.First().Name);
    }

    [Fact]
    public void Create_NullPlanet_ThrowsArgumentNullException()
    {
        using var context = CreateDbContext();
        var repository = new PlanetRepository(context);

        Assert.Throws<ArgumentNullException>(() => repository.Create(null!));
    }

    [Fact]
    public void SaveChanges_ReturnsTrue_WhenSuccessful()
    {
        using var context = CreateDbContext();
        var repository = new PlanetRepository(context);

        var result = repository.SaveChanges();

        Assert.True(result);
    }
}
