using Microsoft.EntityFrameworkCore;
using PlanetService.AsyncDataServices;
using PlanetService.AsyncDataServices.Abstract;
using PlanetService.Data;
using PlanetService.Data.Abstract;
using PlanetService.SyncDataServices.Grpc;
using PlanetService.SyncDataServices.Http;
using PlanetService.SyncDataServices.Http.Abstract;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    Console.WriteLine($"==> Using MS SQL Server");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlanetDbConnection"));
    });
}
else
{
    Console.WriteLine($"==> Using InMemory DB");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase("PlanetDb");
    });
}

builder.Services.AddScoped<IPlanetRepository, PlanetRepository>();
builder.Services.AddHttpClient<IHttpDataClient, HttpDataClient>();
builder.Services.AddSingleton<IMessageBusDataClient, MessageBusDataClient>();
builder.Services.AddGrpc();
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "PlanetService v1"); });
}

Console.WriteLine($"==> SatelliteService endpoint - {app.Configuration["SatelliteService"]}");

app.UseRouting();
app.MapControllers();
app.MapGrpcService<GrpcPlanetService>();
app.MapGet("/protos/planets.proto", async context =>
{
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/planets.proto"));
});

DbSeeder.Seed(app, app.Environment.IsProduction());
app.Run();