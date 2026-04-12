using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.EventProcessing;
using SatelliteService.Models;

namespace SatelliteService.Tests;

public class EventProcessorTests
{
    private readonly Mock<ISatelliteRepository> _repositoryMock = new();
    private readonly EventProcessor _processor;

    public EventProcessorTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _repositoryMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var loggerMock = new Mock<ILogger<EventProcessor>>();

        _processor = new EventProcessor(scopeFactory, loggerMock.Object);
    }

    [Fact]
    public void ProcessEvent_PlanetPublished_CreatesPlanet()
    {
        var dto = new PlanetPublishedDto { Id = 1, Name = "Earth", Event = MessageBusConstants.PlanetPublishedEvent };
        var message = JsonSerializer.Serialize(dto);
        _repositoryMock.Setup(r => r.IsExternalPlanetExists(1)).Returns(false);

        _processor.ProcessEvent(message);

        _repositoryMock.Verify(r => r.CreatePlanet(It.Is<Planet>(p => p.ExternalId == 1)), Times.Once);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void ProcessEvent_DuplicatePlanet_DoesNotCreate()
    {
        var dto = new PlanetPublishedDto { Id = 1, Name = "Earth", Event = MessageBusConstants.PlanetPublishedEvent };
        var message = JsonSerializer.Serialize(dto);
        _repositoryMock.Setup(r => r.IsExternalPlanetExists(1)).Returns(true);

        _processor.ProcessEvent(message);

        _repositoryMock.Verify(r => r.CreatePlanet(It.IsAny<Planet>()), Times.Never);
    }

    [Fact]
    public void ProcessEvent_UnknownEvent_DoesNotCreatePlanet()
    {
        var message = JsonSerializer.Serialize(new { Event = "Unknown_Event" });

        _processor.ProcessEvent(message);

        _repositoryMock.Verify(r => r.CreatePlanet(It.IsAny<Planet>()), Times.Never);
    }
}
