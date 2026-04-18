using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PlanetsSatellites.Web.Models;

namespace PlanetsSatellites.Web.Services;

public sealed record ApiResult<T>(bool Success, T? Value, string? Error)
{
    public static ApiResult<T> Ok(T value) => new(true, value, null);
    public static ApiResult<T> Fail(string error) => new(false, default, error);
}

public sealed class AuthApiClient(HttpClient http)
{
    public async Task<ApiResult<string>> RegisterAsync(RegisterRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/register", request);
        return response.IsSuccessStatusCode
            ? ApiResult<string>.Ok("Registration successful.")
            : ApiResult<string>.Fail(await ReadError(response, "Registration failed."));
    }

    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return ApiResult<LoginResponse>.Fail(await ReadError(response, "Invalid email or password."));

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body is null
            ? ApiResult<LoginResponse>.Fail("Empty response from server.")
            : ApiResult<LoginResponse>.Ok(body);
    }

    private static readonly JsonSerializerOptions ErrorJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static async Task<string> ReadError(HttpResponseMessage response, string fallback)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body)) return fallback;

            var trimmed = body.TrimStart();

            // Identity errors come back as [ { code, description }, ... ]
            if (trimmed.StartsWith('['))
            {
                var errors = JsonSerializer.Deserialize<List<IdentityErrorDto>>(body, ErrorJsonOptions);
                if (errors is { Count: > 0 })
                    return string.Join(" ", errors.Select(e => e.Description).Where(d => !string.IsNullOrWhiteSpace(d)));
            }

            // AuthController.Login returns { "message": "..." }
            if (trimmed.StartsWith('{'))
            {
                var envelope = JsonSerializer.Deserialize<MessageEnvelope>(body, ErrorJsonOptions);
                if (!string.IsNullOrWhiteSpace(envelope?.Message))
                    return envelope.Message!;
            }

            return body;
        }
        catch
        {
            return fallback;
        }
    }

    private sealed class IdentityErrorDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
    }

    private sealed class MessageEnvelope
    {
        public string? Message { get; set; }
    }
}

public sealed class PlanetApiClient(HttpClient http)
{
    public async Task<ApiResult<IReadOnlyList<PlanetDto>>> GetAllAsync()
    {
        try
        {
            var list = await http.GetFromJsonAsync<List<PlanetDto>>("api/planet");
            return ApiResult<IReadOnlyList<PlanetDto>>.Ok(list ?? []);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return ApiResult<IReadOnlyList<PlanetDto>>.Fail("Unauthorized. Please log in.");
        }
        catch (Exception ex)
        {
            return ApiResult<IReadOnlyList<PlanetDto>>.Fail(ex.Message);
        }
    }

    public async Task<ApiResult<PlanetDto>> GetByIdAsync(int id)
    {
        try
        {
            var planet = await http.GetFromJsonAsync<PlanetDto>($"api/planet/{id}");
            return planet is null
                ? ApiResult<PlanetDto>.Fail($"Planet {id} not found.")
                : ApiResult<PlanetDto>.Ok(planet);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return ApiResult<PlanetDto>.Fail($"Planet {id} not found.");
        }
        catch (Exception ex)
        {
            return ApiResult<PlanetDto>.Fail(ex.Message);
        }
    }

    public async Task<ApiResult<PlanetDto>> CreateAsync(CreatePlanetRequest request)
    {
        var response = await http.PostAsJsonAsync("api/planet", request);
        if (!response.IsSuccessStatusCode)
            return ApiResult<PlanetDto>.Fail($"Create failed: {response.StatusCode}");

        var planet = await response.Content.ReadFromJsonAsync<PlanetDto>();
        return planet is null
            ? ApiResult<PlanetDto>.Fail("Empty response.")
            : ApiResult<PlanetDto>.Ok(planet);
    }
}

public sealed class SatelliteApiClient(HttpClient http)
{
    public async Task<ApiResult<IReadOnlyList<ReplicatedPlanetDto>>> GetReplicatedPlanetsAsync()
    {
        try
        {
            var list = await http.GetFromJsonAsync<List<ReplicatedPlanetDto>>("api/s/planet");
            return ApiResult<IReadOnlyList<ReplicatedPlanetDto>>.Ok(list ?? []);
        }
        catch (Exception ex)
        {
            return ApiResult<IReadOnlyList<ReplicatedPlanetDto>>.Fail(ex.Message);
        }
    }

    public async Task<ApiResult<IReadOnlyList<SatelliteDto>>> GetForPlanetAsync(int planetId)
    {
        try
        {
            var list = await http.GetFromJsonAsync<List<SatelliteDto>>(
                $"api/s/planets/{planetId}/satellite");
            return ApiResult<IReadOnlyList<SatelliteDto>>.Ok(list ?? []);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return ApiResult<IReadOnlyList<SatelliteDto>>.Ok([]);
        }
        catch (Exception ex)
        {
            return ApiResult<IReadOnlyList<SatelliteDto>>.Fail(ex.Message);
        }
    }

    public async Task<ApiResult<SatelliteDto>> CreateAsync(int planetId, CreateSatelliteRequest request)
    {
        var response = await http.PostAsJsonAsync(
            $"api/s/planets/{planetId}/satellite", request);

        if (!response.IsSuccessStatusCode)
            return ApiResult<SatelliteDto>.Fail($"Create failed: {response.StatusCode}");

        var sat = await response.Content.ReadFromJsonAsync<SatelliteDto>();
        return sat is null
            ? ApiResult<SatelliteDto>.Fail("Empty response.")
            : ApiResult<SatelliteDto>.Ok(sat);
    }
}
