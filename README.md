# Vicgital.Core.Logging

A .NET shared library that provides structured, JSON-based logging built on top of [Serilog](https://serilog.net/) for ASP.NET Core applications. It offers ready-to-use extensions for configuring loggers, redacting sensitive data, tracking correlation IDs across requests, and shipping logs to sinks like the console, files, or Azure Application Insights.

## Features

- **Preconfigured Serilog setup** — sensible defaults (console JSON output, machine name, process ID, thread ID, and environment enrichers) via `LoggerConfigurationBuilder`.
- **Configuration-driven logging** — build a `LoggerConfiguration` directly from `IConfiguration` (e.g. `appsettings.json`).
- **Sensitive data redaction** — `RedactEnricher` replaces configured property values (e.g. `Password`, `Token`) with `***REDACTED***` before they reach a sink.
- **Correlation ID middleware** — `CorrelationIdMiddleware` reads or generates an `X-Correlation-Id` per request, adds it to the response headers, and pushes it into the Serilog log context.
- **Request logging enrichment** — `UseDefaultSerilogRequestLogging` adds correlation ID, authenticated user, and client IP to each request log.
- **Per-source log level overrides** — fine-tune verbosity for specific namespaces/classes without changing the global minimum level.
- **Multiple sinks** — write to Console, a rolling file, and/or Azure Application Insights.

## Requirements

- .NET 10.0
- ASP.NET Core (for the middleware and application builder extensions)

## Installation

This package is published to a private GitHub Packages feed. A `nuget.config` is included in the repository, pointing to both `nuget.org` and the `vicgital` GitHub Packages feed.

```xml
<packageSources>
  <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  <add key="github" value="https://nuget.pkg.github.com/vicgital/index.json" />
</packageSources>
```

Authentication to the GitHub feed uses a read-only personal access token supplied via the `GIT_PACKAGES_READ_ONLY_PAT` environment variable.

Then add the package reference:

```bash
dotnet add package Vicgital.Core.Logging
```

## Usage

### 1. Build a logger configuration

Use the default configuration:

```csharp
using Vicgital.Core.Logging.Serilog.Configuration;
using Serilog.Events;

var loggerConfiguration = LoggerConfigurationBuilder.BuildDefault(LogEventLevel.Information);
```

Or build one from `IConfiguration` (e.g. reading a `Serilog` section from `appsettings.json`):

```csharp
var loggerConfiguration = LoggerConfigurationBuilder.BuildFromConfiguration(configuration);
```

### 2. Customize the configuration

```csharp
using Vicgital.Core.Logging.Serilog.Configuration.Extensions;
using Serilog.Events;

loggerConfiguration
    .OverrideMinimumLevel("Microsoft.AspNetCore", LogEventLevel.Warning)
    .EnrichWithRedacter(["Password", "Token", "SSN"])
    .WriteToApplicationInsights(appInsightsConnectionString)
    .WriteToFile("logs/log-.json", RollingInterval.Day);
```

### 3. Wire it into the host and DI container

```csharp
using Vicgital.Core.Logging.Serilog.Extensions;
using Serilog;

builder.Host.UseSerilog(loggerConfiguration);

var logger = loggerConfiguration.CreateLogger();
builder.Services.AddSerilogLogging(logger);
```

### 4. Add correlation ID tracking and request logging

```csharp
using Vicgital.Core.Logging.Serilog.Extensions;
using Vicgital.Core.Logging.Serilog.Middlewares;

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseDefaultSerilogRequestLogging();
```

Each request will automatically include a `CorrelationId` (from the `X-Correlation-Id` header, or generated from the ASP.NET Core trace identifier if absent) in its log entries, along with the request path, status code, elapsed time, and authenticated user (if any).

## Project Structure

```
src/Vicgital.Core.Logging/
├── Serilog/
│   ├── Configuration/
│   │   ├── LoggerConfigurationBuilder.cs          # Default / IConfiguration-based setup
│   │   └── Extensions/
│   │       └── LoggerConfigurationBuilderExtensions.cs  # Redaction, sinks, level overrides
│   ├── Enrichers/
│   │   └── RedactEnricher.cs                      # Redacts sensitive properties
│   ├── Extensions/
│   │   ├── HostBuilderExtensions.cs                # IHostBuilder.UseSerilog
│   │   ├── ServiceCollectionExtensions.cs           # IServiceCollection.AddSerilogLogging
│   │   └── ApplicationBuilderExtensions.cs          # Default request logging
│   └── Middlewares/
│       └── CorrelationIdMiddleware.cs              # Correlation ID propagation
```

## Dependencies

- `Serilog.AspNetCore`
- `Serilog.Enrichers.Environment`
- `Serilog.Enrichers.Process`
- `Serilog.Enrichers.Thread`
- `Serilog.Sinks.ApplicationInsights`
- `Microsoft.Extensions.Configuration`


