using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace Vicgital.Core.Logging.Serilog.Configuration
{
    /// <summary>
    /// Helper class to build Serilog LoggerConfiguration from IConfiguration or with default settings.
    /// </summary>
    public static class LoggerConfigurationBuilder
    {
        /// <summary>
        /// Creates a new LoggerConfiguration instance using settings from the specified configuration source.
        /// </summary>
        /// <remarks>Use this method to configure logging based on application configuration files, such
        /// as appsettings.json or environment variables. The returned LoggerConfiguration can be further customized
        /// before creating a logger.</remarks>
        /// <param name="config">The configuration source containing logger settings. Must not be null.</param>
        /// <returns>A LoggerConfiguration instance initialized with settings from the provided configuration.</returns>
        public static LoggerConfiguration BuildFromConfiguration(IConfiguration config)
        {
            var loggerConfiguration = new LoggerConfiguration();
            loggerConfiguration.ReadFrom.Configuration(config);
            return loggerConfiguration;
        }

        /// <summary>
        /// Creates a default Serilog logger configuration with standard enrichers and console output, using the
        /// specified minimum log event level.
        /// </summary>
        /// <remarks>The returned configuration includes enrichers for log context, machine name, process
        /// ID, thread ID, and the 'ASPNETCORE_ENVIRONMENT' environment variable. Output is written to the console in
        /// JSON format.</remarks>
        /// <param name="minimumLogLevel">The minimum level of log events to be captured by the logger. Events below this level are ignored.</param>
        /// <returns>A configured LoggerConfiguration instance with the specified minimum log level, standard enrichers, and
        /// JSON-formatted console output.</returns>
        public static LoggerConfiguration BuildDefault(LogEventLevel minimumLogLevel)
        {
            var loggerConfiguration = new LoggerConfiguration();

            switch (minimumLogLevel)
            {
                case LogEventLevel.Verbose:
                    loggerConfiguration.MinimumLevel.Verbose();
                    break;
                case LogEventLevel.Debug:
                    loggerConfiguration.MinimumLevel.Debug();
                    break;
                case LogEventLevel.Information:
                    loggerConfiguration.MinimumLevel.Information();
                    break;
                case LogEventLevel.Warning:
                    loggerConfiguration.MinimumLevel.Warning();
                    break;
                case LogEventLevel.Error:
                    loggerConfiguration.MinimumLevel.Error();
                    break;
                case LogEventLevel.Fatal:
                    loggerConfiguration.MinimumLevel.Fatal();
                    break;
                default:
                    break;
            }

            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentVariable(propertyName: "Environment", environmentVariableName: "ASPNETCORE_ENVIRONMENT")
                .WriteTo.Console(new JsonFormatter());

            return loggerConfiguration;

        }

    }
}
