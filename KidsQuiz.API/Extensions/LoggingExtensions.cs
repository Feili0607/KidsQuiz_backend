using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;

namespace KidsQuiz.API.Extensions
{
    public static class LoggingExtensions
    {
        public static IHostBuilder ConfigureLogging(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            var instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
            var logLevel = configuration.GetValue<string>("Logging:LogLevel:Default") ?? "Information";

            return hostBuilder.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Is(Enum.Parse<LogEventLevel>(logLevel))
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "KidsQuiz")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/kidsquiz-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7)
                .WriteTo.ApplicationInsights(
                    new TelemetryConfiguration { InstrumentationKey = instrumentationKey },
                    new TraceTelemetryConverter()));
        }

        public static IServiceCollection AddApplicationInsightsTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = configuration["ApplicationInsights:ConnectionString"];
                options.EnableAdaptiveSampling = false;
                options.EnableQuickPulseMetricStream = true;
                options.EnablePerformanceCounterCollectionModule = true;
                options.EnableRequestTrackingTelemetryModule = true;
                options.EnableDependencyTrackingTelemetryModule = true;
                options.EnableEventCounterCollectionModule = true;
            });

            services.AddApplicationInsightsKubernetesEnricher();

            return services;
        }
    }
} 