using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KidsQuiz.API.Extensions;
using KidsQuiz.Services.Caching;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Host.ConfigureLogging(builder.Configuration);

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);

// Add caching
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Use custom middleware
app.UseCustomMiddleware();

app.MapControllers();

try
{
    Log.Information("Starting KidsQuiz API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "KidsQuiz API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
