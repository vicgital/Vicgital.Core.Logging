using Microsoft.Extensions.Hosting;
using Serilog;
using Vicgital.Core.Logging.Serilog.Extensions;

namespace Vicgital.Core.Logging.Serilog.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IHostBuilder"/> to configure Serilog as the logging provider.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Configures Serilog as the logging provider for the specified host builder using the provided logger
        /// configuration.
        /// </summary>
        /// <remarks>This method enriches log events with the application name from the hosting
        /// environment. It should be called before building the host to ensure Serilog is used for all
        /// logging.</remarks>
        /// <param name="host">The host builder to configure with Serilog logging.</param>
        /// <param name="cfg">The Serilog logger configuration to use for setting up logging.</param>
        /// <returns>The host builder instance configured to use Serilog for logging.</returns>
        public static IHostBuilder UseSerilog(this IHostBuilder host, LoggerConfiguration cfg)
        {
            return host.UseSerilog((context, configuration) =>
            {
                configuration = cfg.Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName);
            });

        }
    }
}
