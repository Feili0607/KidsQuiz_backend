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
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "AllowFrontend";

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName,
        policy => policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Configure logging
builder.Host.ConfigureLogging(builder.Configuration);

// Add Azure AD Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add Authorization with roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("KidOnly", policy => policy.RequireRole("Kid"));
    options.AddPolicy("ParentOrGuardian", policy => policy.RequireRole("Parent", "Guardian"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ParentOrAdmin", policy => policy.RequireRole("Parent", "Guardian", "Admin"));
});

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);

// Add caching
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();

// Add Application Insights
KidsQuiz.API.Extensions.LoggingExtensions.AddApplicationInsightsTelemetry(builder.Services, builder.Configuration);


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
builder.Services.AddHttpClient();
builder.Services.AddScoped<ILLMQuizService, LLMQuizService>();
builder.Services.AddScoped<IRewardService, RewardService>();
builder.Services.AddScoped<IGuardianService, GuardianService>();

var app = builder.Build();

app.UseCors(CorsPolicyName);

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KidsQuiz API V1");
    //c.RoutePrefix = /swagger/v1; // Optional: Serve Swagger UI at root "/"
});


//app.UseHttpsRedirection();

// Add authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

// Use custom middleware
app.UseCustomMiddleware();

app.MapControllers();

try
{
    Log.Information("Starting KidsQuiz API");
    app.MapGet("/", () => Results.Ok("KidsQuiz API is running."));
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

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

