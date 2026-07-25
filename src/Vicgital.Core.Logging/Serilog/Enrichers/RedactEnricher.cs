using Serilog.Core;
using Serilog.Events;

namespace Vicgital.Core.Logging.Serilog.Enrichers
{

    /// <summary>
    /// Serilog Enricher that redacts specified fields from log events by replacing their values with a placeholder.
    /// </summary>
    /// <param name="fields"></param>
    public sealed class RedactEnricher(string[] fields) : ILogEventEnricher
    {
        private readonly HashSet<string> _fields = [.. fields.Select(f => f.ToLowerInvariant())];

        /// <summary>
        /// Redacts sensitive properties in the specified log event by replacing their values with a placeholder.
        /// </summary>
        /// <remarks>This method scans the properties of the provided log event and replaces the values of
        /// any properties whose keys match a predefined set of sensitive field names. The original property values are
        /// not preserved. Use this method to prevent sensitive information from being logged.</remarks>
        /// <param name="logEvent">The log event whose properties will be examined and potentially redacted. Cannot be null.</param>
        /// <param name="factory">The factory used to create new or updated log event properties. Cannot be null.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
        {
            foreach (var prop in logEvent.Properties.ToList())
            {
                if (_fields.Contains(prop.Key.ToLowerInvariant()))
                {
                    logEvent.AddOrUpdateProperty(factory.CreateProperty(prop.Key, "***REDACTED***"));
                }
            }
        }
    }

}
