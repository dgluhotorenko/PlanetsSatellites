using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace PlanetsSatellites.Web.Services;

public sealed class AuthHeaderHandler(ILocalStorageService storage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await storage.GetItemAsStringAsync(
            JwtAuthStateProvider.TokenKey, cancellationToken);

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
