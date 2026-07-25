using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Vicgital.Core.Logging.Serilog.Middlewares
{
    /// <summary>
    /// Middleware to ensure that each HTTP request has a correlation ID for consistent logging across services. It checks for an incoming correlation ID in the request headers and uses it if present; otherwise, it generates a new one using the ASP.NET Core trace identifier
    /// </summary>
    /// <param name="next"></param>
    public sealed class CorrelationIdMiddleware(RequestDelegate next)
    {
        public const string Header = "X-Correlation-Id";

        /// <summary>
        /// Processes the HTTP request by assigning a correlation identifier to the request and response headers, and
        /// ensures the identifier is available in the logging context for the duration of the request.
        /// </summary>
        /// <remarks>If the incoming request contains a correlation identifier header, its value is used;
        /// otherwise, a new identifier is generated from the request's trace identifier. The correlation identifier is
        /// added to the response headers and made available to logging frameworks for improved traceability across
        /// distributed systems.</remarks>
        /// <param name="ctx">The HTTP context for the current request. Provides access to request and response data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Invoke(HttpContext ctx)
        {
            var id = ctx.Request.Headers.TryGetValue(Header, out var v) && !string.IsNullOrWhiteSpace(v)
                ? v.ToString()
                : ctx.TraceIdentifier; // ASP.NET sets one by default
            ctx.Response.Headers[Header] = id;
            using (LogContext.PushProperty("CorrelationId", id))
            {
                await next(ctx);
            }
        }
    }

}
