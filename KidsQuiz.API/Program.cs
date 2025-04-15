using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using KidsQuiz.API.Extensions;
using KidsQuiz.Services.Caching;
using Serilog;
using Microsoft.EntityFrameworkCore;
using KidsQuiz.Data;
using KidsQuiz.Services.Interfaces;
using KidsQuiz.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Host.ConfigureLogging(builder.Configuration);

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);

// Add caching
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

// Add Application Insights
Microsoft.Extensions.DependencyInjection.ApplicationInsightsExtensions.AddApplicationInsightsTelemetry(builder.Services, builder.Configuration);

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<IKidService, KidService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IQuizSolvingRecordService, QuizSolvingRecordService>();

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
