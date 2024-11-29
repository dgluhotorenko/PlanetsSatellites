using Microsoft.EntityFrameworkCore;
using SatelliteService.AsyncDataServices;
using SatelliteService.Data;
using SatelliteService.Data.Abstract;
using SatelliteService.EventProcessing;
using SatelliteService.EventProcessing.Abstract;
using SatelliteService.SyncDataServices.Grpc;
using SatelliteService.SyncDataServices.Grpc.Abstract;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("SatelliteDb"); });
builder.Services.AddScoped<ISatelliteRepository, SatelliteRepository>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<IPlanetDataClient, PlanetDataClient>();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.WebHost.UseUrls("http://0.0.0.0:6000");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "SatelliteService v1"); });
}

app.UseRouting();
app.MapControllers();
PrepDb.PrepPopulation(app);
app.Run();