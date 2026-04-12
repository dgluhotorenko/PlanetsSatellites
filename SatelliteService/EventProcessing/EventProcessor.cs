using System.Text.Json;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.EventProcessing.Abstract;
using SatelliteService.Mappers;

namespace SatelliteService.EventProcessing;

public class EventProcessor(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EventProcessor> logger) : IEventProcessor
{
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEventType(message);

        switch (eventType)
        {
            case EventType.PlanetPublished:
                AddPlanet(message);
                break;
            case EventType.Undetermined:
                logger.LogWarning("Received undetermined event type");
                break;
        }
    }

    private EventType DetermineEventType(string message)
    {
        var genericEvent = JsonSerializer.Deserialize<GenericEventDto>(message);

        return genericEvent?.Event == MessageBusConstants.PlanetPublishedEvent
            ? EventType.PlanetPublished
            : EventType.Undetermined;
    }

    private void AddPlanet(string message)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ISatelliteRepository>();
        var dto = JsonSerializer.Deserialize<PlanetPublishedDto>(message);

        if (dto is null)
        {
            logger.LogWarning("Failed to deserialize PlanetPublishedDto from message");
            return;
        }

        var planet = dto.ToModel();

        if (repository.IsExternalPlanetExists(planet.ExternalId))
        {
            logger.LogInformation("Planet with ExternalId {ExternalId} already exists — skipping",
                planet.ExternalId);
            return;
        }

        repository.CreatePlanet(planet);
        repository.SaveChanges();
        logger.LogInformation("Planet '{Name}' (ExternalId: {ExternalId}) added via message bus",
            planet.Name, planet.ExternalId);
    }
}
