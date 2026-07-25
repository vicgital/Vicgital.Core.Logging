using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
namespace Vicgital.Core.Logging.Serilog.Configuration.Extensions
{
    /// <summary>
    /// Extension methods for LoggerConfiguration to provide additional configuration options
    /// </summary>
    public static class LoggerConfigurationBuilderExtensions
    {
        /// <summary>
        /// Overrides the minimum log event level for a specified source within the logger configuration.
        /// </summary>
        /// <remarks>Use this method to set a different minimum log level for a particular source, such as
        /// a namespace or class, without affecting the global minimum level. This is useful for increasing or
        /// decreasing verbosity for specific components.</remarks>
        /// <param name="loggerConfiguration">The logger configuration to apply the override to.</param>
        /// <param name="source">The name of the source for which to override the minimum log event level. Cannot be null or whitespace.</param>
        /// <param name="minimumLevel">The minimum log event level to apply to the specified source.</param>
        /// <returns>A logger configuration with the minimum level override applied for the specified source.</returns>
        public static LoggerConfiguration OverrideMinimumLevel(this LoggerConfiguration loggerConfiguration, string source, LogEventLevel minimumLevel)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(source, nameof(source));
            return loggerConfiguration.MinimumLevel.Override(source, minimumLevel);
        }

        /// <summary>
        /// Enriches the logger configuration with a redacter that redacts the specified properties from log events.
        /// </summary>
        /// <remarks>Use this method to automatically redact sensitive information from log events by
        /// specifying the property names to be redacted. This is useful for ensuring that confidential data does not
        /// appear in logs.</remarks>
        /// <param name="loggerConfiguration">The logger configuration to enrich. Cannot be null.</param>
        /// <param name="propertiesToRedact">A list of property names to redact from log events. Cannot be null or empty.</param>
        /// <returns>The logger configuration enriched with a redacter for the specified properties.</returns>
        /// <exception cref="Exception">Thrown if propertiesToRedact is null or empty.</exception>
        public static LoggerConfiguration EnrichWithRedacter(this LoggerConfiguration loggerConfiguration, List<string> propertiesToRedact)
        {
            if (propertiesToRedact is null or { Count: 0 })
                throw new Exception("Properties to redact cannot be null or empty.");

            return loggerConfiguration.Enrich.With(new Enrichers.RedactEnricher([.. propertiesToRedact]));
        }


        /// <summary>
        /// Configures the logger to send log events to Azure Application Insights using the specified connection
        /// string.
        /// </summary>
        /// <remarks>This extension method adds the Application Insights sink to the logger configuration,
        /// allowing log events to be sent to the specified Application Insights resource as traces. The method uses the
        /// default telemetry converter for trace events.</remarks>
        /// <param name="loggerConfiguration">The logger configuration to which the Application Insights sink will be added.</param>
        /// <param name="appInsightsConnectionString">The connection string for the Azure Application Insights resource. Cannot be null or whitespace.</param>
        /// <returns>The logger configuration with Application Insights logging enabled.</returns>
        public static LoggerConfiguration WriteToApplicationInsights(this LoggerConfiguration loggerConfiguration, string appInsightsConnectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(appInsightsConnectionString, nameof(appInsightsConnectionString));
            return loggerConfiguration.WriteTo.ApplicationInsights(appInsightsConnectionString, TelemetryConverter.Traces);
        }



        /// <summary>
        /// Configures the logger to write log events to a file at the specified path with the given rolling interval.
        /// </summary>s
        /// <remarks>This method enables file-based logging with support for rolling log files based on
        /// the specified interval. Use this to persist logs to disk and manage log file size and retention.</remarks>
        /// <param name="loggerConfiguration">The logger configuration to apply the file sink to.</param>
        /// <param name="path">The file path where log events will be written. Cannot be null, empty, or consist only of white-space
        /// characters.</param>
        /// <param name="rollingInterval">The interval at which the log file will roll over to a new file.</param>
        /// <returns>The logger configuration, allowing further configuration to be chained.</returns>
        public static LoggerConfiguration WriteToFile(this LoggerConfiguration loggerConfiguration, string path, RollingInterval rollingInterval)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path, nameof(path));
            return loggerConfiguration.WriteTo.File(new JsonFormatter(), path, rollingInterval: rollingInterval);
        }

    }
}
