using Microsoft.EntityFrameworkCore;
using SatelliteService.Data;
using SatelliteService.Data.Abstract;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => { options.UseInMemoryDatabase("SatelliteDb"); });
builder.Services.AddScoped<ISatelliteRepository, SatelliteRepository>();
builder.WebHost.UseUrls("http://0.0.0.0:6000");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/v1/swagger.json", "SatelliteService v1"); });
}

app.UseRouting();
// app.UseAuthorization();
app.MapControllers();
app.Run();