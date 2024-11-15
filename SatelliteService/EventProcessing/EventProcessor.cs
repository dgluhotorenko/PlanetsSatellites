using System.Text.Json;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.EventProcessing.Abstract;
using SatelliteService.Mappers;

namespace SatelliteService.EventProcessing;

public class EventProcessor(IServiceScopeFactory serviceScopeFactory) : IEventProcessor
{
    public void ProcessEvent(string message)
    {
        var eventType = DetermineEventType(message);
        
        switch (eventType)
        {
            case EventType.PlanetPublished:
                Console.WriteLine("==> Event type is PlanetPublished.");
                break;
            case EventType.Undetermined:
                Console.WriteLine("==> Event type is Undetermined.");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private EventType DetermineEventType(string message)
    {
        Console.WriteLine("==> Determining event type...");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);

        return eventType?.Event == "Planet_Published" ? EventType.PlanetPublished : EventType.Undetermined;
    }
    
    private void AddPlanet(string message)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var satelliteRepository = scope.ServiceProvider.GetRequiredService<ISatelliteRepository>();
            var planetPublishedDto = JsonSerializer.Deserialize<PlanetPublishedDto>(message);

            try
            {
                var planet = planetPublishedDto.ToModel();
                if (!satelliteRepository.IsExternalPlanetExists(planet.ExternalId))
                {
                    satelliteRepository.CreatePlanet(planet);
                    satelliteRepository.SaveChanges();
                }
                else
                {
                    Console.WriteLine("==> Planet already exists in DB.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("==> Error adding planet to DB: " + e.Message);
                throw;
            }
        }
    }
}