using System.Text;
using System.Text.Json;
using PlanetService.DTOs;

namespace PlanetService.SyncDataServices.Http;

public class HttpPlanetDataClient(HttpClient httpClient, IConfiguration configuration) : IPlanetDataClient
{
    public async Task SendPlanetDataAsync(PlanetReadDto planet)
    {
        var content = new StringContent(JsonSerializer.Serialize(planet), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{configuration["SatelliteService"]}", content);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "==> POST to SatelliteService was OK!"
            : "==> POST to SatelliteService was not OK!");
    }
}