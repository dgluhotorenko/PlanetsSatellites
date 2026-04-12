using System.Text;
using System.Text.Json;
using PlanetService.DTOs;
using PlanetService.SyncDataServices.Http.Abstract;

namespace PlanetService.SyncDataServices.Http;

public class HttpDataClient(
    HttpClient httpClient,
    IConfiguration configuration,
    ILogger<HttpDataClient> logger) : IHttpDataClient
{
    public async Task SendPlanetDataAsync(PlanetReadDto planet)
    {
        var endpoint = configuration["SatelliteService"]
            ?? throw new InvalidOperationException("SatelliteService endpoint is not configured");

        var content = new StringContent(
            JsonSerializer.Serialize(planet), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
            logger.LogInformation("Synchronous POST to SatelliteService succeeded");
        else
            logger.LogWarning("Synchronous POST to SatelliteService failed with status {StatusCode}",
                response.StatusCode);
    }
}
