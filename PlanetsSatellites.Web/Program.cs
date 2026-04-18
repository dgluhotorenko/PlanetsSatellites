using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using PlanetsSatellites.Web;
using PlanetsSatellites.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
    config.SnackbarConfiguration.HideTransitionDuration = 200;
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
});
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<JwtAuthStateProvider>());

builder.Services.AddScoped<AuthHeaderHandler>();

var apis = builder.Configuration.GetSection("Apis");
var authUrl = apis["Auth"] ?? "http://localhost:7000";
var planetUrl = apis["Planet"] ?? "http://localhost:5000";
var satelliteUrl = apis["Satellite"] ?? "http://localhost:6001";

builder.Services.AddHttpClient<AuthApiClient>(c => c.BaseAddress = new Uri(authUrl));
builder.Services.AddHttpClient<PlanetApiClient>(c => c.BaseAddress = new Uri(planetUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>();
builder.Services.AddHttpClient<SatelliteApiClient>(c => c.BaseAddress = new Uri(satelliteUrl))
    .AddHttpMessageHandler<AuthHeaderHandler>();

await builder.Build().RunAsync();
