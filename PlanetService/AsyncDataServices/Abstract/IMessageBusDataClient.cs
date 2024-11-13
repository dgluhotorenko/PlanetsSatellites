using PlanetService.DTOs;

namespace PlanetService.AsyncDataServices.Abstract;

public interface IMessageBusDataClient
{
    Task PublishNewPlanetAsync(PlanetPublishedDto planetPublishedDto);

    Task InitializeAsync();

    Task DisposeAsync();
}