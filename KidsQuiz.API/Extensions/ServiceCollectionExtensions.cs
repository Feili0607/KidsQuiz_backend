using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using KidsQuiz.API.Middleware;

namespace KidsQuiz.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomMiddleware(this IServiceCollection services)
        {
            services.AddScoped<ExceptionHandlingMiddleware>();
            services.AddScoped<RequestLoggingMiddleware>();
            return services;
        }

        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            return app;
        }
    }
} 