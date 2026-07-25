using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Vicgital.Core.Logging.Serilog.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/> to configure Serilog request logging middleware.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {

        /// <summary>
        /// Enables default Serilog request logging middleware for the application, enriching log events with
        /// correlation ID, user identity, and client IP address.
        /// </summary>
        /// <remarks>This extension method configures Serilog to log HTTP requests with additional
        /// diagnostic context, including correlation ID, user name (if authenticated), and client IP address. Use this
        /// method to quickly add structured request logging to an ASP.NET Core application.</remarks>
        /// <param name="app">The application builder used to configure the request pipeline. Cannot be null.</param>
        public static void UseDefaultSerilogRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                // Each request: { requestPath, statusCode, elapsedMs, correlationId, user }
                opts.EnrichDiagnosticContext = (diag, http) =>
                {
                    diag.Set("CorrelationId", http.TraceIdentifier);
                    diag.Set("UserId", http.User?.Identity?.IsAuthenticated == true
                        ? http.User.Identity!.Name
                        : null);
                    diag.Set("ClientIP", http.Connection.RemoteIpAddress?.ToString());
                };
            });

        }
    }
}
