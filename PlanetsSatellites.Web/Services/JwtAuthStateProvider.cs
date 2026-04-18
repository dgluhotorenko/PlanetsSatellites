using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace PlanetsSatellites.Web.Services;

public sealed class JwtAuthStateProvider(ILocalStorageService storage) : AuthenticationStateProvider
{
    public const string TokenKey = "auth_token";
    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await storage.GetItemAsStringAsync(TokenKey);
        if (string.IsNullOrWhiteSpace(token) || IsExpired(token))
        {
            if (!string.IsNullOrWhiteSpace(token))
                await storage.RemoveItemAsync(TokenKey);
            return Anonymous;
        }

        // JwtSecurityTokenHandler serialises ClaimTypes.Name as the short "unique_name" claim.
        // Setting nameType here so AuthorizeView's context.User.Identity.Name resolves correctly.
        var identity = new ClaimsIdentity(
            ParseClaims(token),
            authenticationType: "jwt",
            nameType: "unique_name",
            roleType: "role");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await storage.SetItemAsStringAsync(TokenKey, token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await storage.RemoveItemAsync(TokenKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static IEnumerable<Claim> ParseClaims(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.CanReadToken(token)
            ? handler.ReadJwtToken(token).Claims
            : [];
    }

    private static bool IsExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return true;
        var jwt = handler.ReadJwtToken(token);
        return jwt.ValidTo < DateTime.UtcNow.AddSeconds(30);
    }
}
