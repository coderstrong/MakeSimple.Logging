using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MakeSimple.Logging
{
    public static class MakeSimpleLoggingExtentions
    {
        public static void AddMakeSimpleLoging(this IServiceCollection services, LoggingOption options = null)
        {
            if (options == null)
            {
                options = new LoggingOption();
            }

            var loggerConfig = new LoggerConfiguration()
                .WriteTo
                .Map(evt => evt.Level, (level, wt) =>
                    wt.File(new LoggingFormat(), options.Path + "/Logs/" + level + "-.log", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: options.FileSizeLimit)
                ).Enrich.FromLogContext();

            if (options.IsEnableTracing)
            {
                loggerConfig = loggerConfig.Enrich.With<ActivityEnricher>();
            }

            if (options.IsOffLogSystem)
            {
                loggerConfig = loggerConfig.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning);
            }

            if (options.IsLogConsole)
            {
                loggerConfig = loggerConfig.WriteTo
                .Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message}{NewLine}{Exception}");
            }

            loggerConfig = options.MinimumLevel switch
            {
                LoggerLevel.Information => loggerConfig.MinimumLevel.Information(),
                LoggerLevel.Verbose => loggerConfig.MinimumLevel.Verbose(),
                LoggerLevel.Debug => loggerConfig.MinimumLevel.Debug(),
                LoggerLevel.Error => loggerConfig.MinimumLevel.Error(),
                LoggerLevel.Fatal => loggerConfig.MinimumLevel.Fatal(),
                LoggerLevel.Warning => loggerConfig.MinimumLevel.Warning(),
                _ => loggerConfig.MinimumLevel.Information(),
            };
            Log.Logger = loggerConfig.CreateLogger();
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory = loggerFactory.AddSerilog(Log.Logger);
            services.AddSingleton<ILoggerFactory>(loggerFactory);
        }

        public static void AddMakeSimpleLoging(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggingMiddleware>();
        }
    }
}