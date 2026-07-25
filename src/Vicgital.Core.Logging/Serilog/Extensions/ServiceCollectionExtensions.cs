using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Vicgital.Core.Logging.Serilog.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring logging services in an IServiceCollection using Serilog.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Serilog-based logging to the specified service collection using the provided logger instance.
        /// </summary>
        /// <remarks>This method enables Serilog as the logging provider for the application's dependency
        /// injection container. Call this method during service configuration to ensure that all logging uses the
        /// specified Serilog logger.</remarks>
        /// <param name="services">The service collection to which logging services are added. Cannot be null.</param>
        /// <param name="logger">The Serilog logger instance to use for logging. Cannot be null.</param>
        public static void AddSerilogLogging(this IServiceCollection services, ILogger logger)
        {
            services.AddLogging(configure =>
            {
                configure.AddSerilog(logger);
            });
        }

    }
}
