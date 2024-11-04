using Microsoft.EntityFrameworkCore;
using SatelliteService.Data;
using SatelliteService.Data.Abstract;
using SatelliteService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsProduction())
{
    Console.WriteLine($"==> Using MS SQL Server");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SatelliteDbConnection"));
    });
}
else
{
    Console.WriteLine($"==> Using InMemory DB");
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase("SatelliteDb");
    });
}

builder.Services.AddScoped<ISatelliteRepository, SatelliteRepository>();
builder.Services.AddHttpClient<IPlanetDataClient, HttpPlanetDataClient>();
builder.WebHost.UseUrls("http://0.0.0.0:5000");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "SatelliteService v1"); });
}

Console.WriteLine($"==> PlanetService endpoint - {app.Configuration["PlanetService"]}");

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

DbSeeder.Seed(app, app.Environment.IsProduction());

app.Run();