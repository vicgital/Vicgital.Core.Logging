using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Compact;

namespace Vicgital.Core.Logging.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Serilog Logging
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="configuration">current app's IConfiguration which includes the "Serilog" section</param>
        /// <returns></returns>
        public static IServiceCollection AddSerilogConsoleLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console(new CompactJsonFormatter())
              .ReadFrom.Configuration(configuration)
              .Enrich.WithSpan()
              .CreateLogger();


            AddLogging(services, configuration);

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services">instance of IServiceCollection</param>
        /// <param name="filePath">the path of the file where logging writes to</param>
        /// <param name="configuration">current app's IConfiguration which includes the "Serilog" section</param>
        /// <returns></returns>
        public static IServiceCollection AddSerilogConsoleAndFileLogging(this IServiceCollection services, string filePath, IConfiguration configuration)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            Log.Logger = new LoggerConfiguration()
              .WriteTo.Console(new CompactJsonFormatter())
              .WriteTo.File(filePath)
              .ReadFrom.Configuration(configuration)
              .Enrich.WithSpan()
              .CreateLogger();

            AddLogging(services, configuration);

            return services;
        }


        private static void AddLogging(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.AddDebug();
                loggingBuilder.AddSerilog();
            });
        }

    }
}
