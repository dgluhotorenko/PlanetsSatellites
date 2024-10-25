using Microsoft.EntityFrameworkCore;
using SatelliteService.Data;
using SatelliteService.Data.Abstract;
using SatelliteService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("SatelliteDb");
});
builder.Services.AddScoped<ISatelliteRepository, SatelliteRepository>();
builder.Services.AddHttpClient<IPlanetDataClient, HttpPlanetDataClient>();
builder.WebHost.UseUrls("http://0.0.0.0:5000 ");

var app = builder.Build();

// Configure the HTTP request pipeline.
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

// pre-seed the database, not for prod
DbSeeder.Seed(app);

app.Run();