using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
builder.Services.AddHealthChecks();

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("PlanetDbConnection")));
}
else
{
    // In production, always use a real database (SQL Server, PostgreSQL, etc.)
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("PlanetDb"));
}

builder.Services.AddScoped<IPlanetRepository, PlanetRepository>();
builder.Services.AddHttpClient<IHttpDataClient, HttpDataClient>();
builder.Services.AddSingleton<IMessageBusDataClient, MessageBusDataClient>();
builder.Services.AddGrpc();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // In production, use Azure Key Vault or a similar secrets manager for JWT configuration
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "PlanetService v1"));
}

// Authentication & authorization must precede endpoint mapping
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlanetService>();
app.MapHealthChecks("/health");
app.MapGet("/protos/planets.proto", async context =>
{
    context.Response.ContentType = "text/plain";
    await context.Response.WriteAsync(await File.ReadAllTextAsync("Protos/planets.proto"));
});

DbSeeder.Seed(app);

app.Run();
