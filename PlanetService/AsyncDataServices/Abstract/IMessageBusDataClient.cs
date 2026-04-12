using PlanetService.DTOs;

namespace PlanetService.AsyncDataServices.Abstract;

public interface IMessageBusDataClient : IAsyncDisposable
{
    Task PublishNewPlanetAsync(PlanetPublishedDto planetPublishedDto);
}
