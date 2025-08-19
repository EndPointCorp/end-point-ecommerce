// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace EndPointEcommerce.Infrastructure.Startup;

public static class LogHelper
{
    private static bool IsHealthCheckEndpoint(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is not null)
        {
            // Note: "Health checks" is the default endpoint name for health checks added via
            // Microsoft.AspNetCore.Builder.HealthCheckEndpointRouteBuilderExtensions
            // (but it is a private const, so we cannot reference it here)
            return string.Equals(endpoint.DisplayName, "Health checks", StringComparison.Ordinal);
        }

        return false;
    }

    /// <summary>
    /// Alternate function for <see cref="Serilog.AspNetCore.RequestLoggingOptions.GetLevel"/> that returns
    /// <see cref="LogEventLevel.Verbose"/> whenever the request is to an ASP.NET health check endpoint
    /// and that health check did not result in an error. This will usually result in these requests
    /// being excluded from log output (unless the default log level is set that high). For all other
    /// successful requests <see cref="LogEventLevel.Information"/> is returned. 
    /// </summary>
    public static LogEventLevel ExcludeHealthChecks(HttpContext httpContext, double _, Exception? ex)
    {
        if (ex != null)
            return LogEventLevel.Error;

        if (httpContext.Response.StatusCode > 499)
            return LogEventLevel.Error;

        return IsHealthCheckEndpoint(httpContext) ? LogEventLevel.Verbose : LogEventLevel.Information;
    }
}