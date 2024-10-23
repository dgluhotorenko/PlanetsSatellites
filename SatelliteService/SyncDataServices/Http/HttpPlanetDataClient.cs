using System.Text;
using System.Text.Json;
using SatelliteService.DTOs;

namespace SatelliteService.SyncDataServices.Http;

public class HttpPlanetDataClient(HttpClient httpClient, IConfiguration configuration) : IPlanetDataClient
{
    public async Task SendSatelliteDataAsync(SatelliteReadDto satellite)
    {
        var content = new StringContent(JsonSerializer.Serialize(satellite), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{configuration["PlanetService"]}", content);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "==> POST to PlanetService was OK!"
            : "==> POST to PlanetService was not OK!");
    }
}