using System;
using CoConnect.Infrastructure.QueryRouting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CoConnect.Infrastructure
{
    public sealed class QueryMiddleware
    {
        private readonly ILogger<QueryMiddleware> _logger;
        private readonly QueryRouteRegistry _routeRegistry;
        private readonly QueryRouteExecutor _routeExecutor;
        private readonly RequestDelegate _next;

        public QueryMiddleware(
            ILogger<QueryMiddleware> logger,
            QueryRouteRegistry routeRegistry,
            QueryRouteExecutor routeExecutor,
            RequestDelegate next)
        {
            _logger = logger;
            _routeRegistry = routeRegistry;
            _routeExecutor = routeExecutor;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (!HttpMethods.IsGet(context.Request.Method) || !context.Request.Path.StartsWithSegments("/api/query", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }

                if (!_routeRegistry.TryMatch(context.Request.Method, context.Request.Path.Value ?? string.Empty, out var match))
                {
                    await _next(context);
                    return;
                }

                _logger.LogInformation("Processing query route {Method} {Path}", context.Request.Method, context.Request.Path);

                var result = await _routeExecutor.ExecuteAsync(match, context.RequestAborted).ConfigureAwait(false);
                context.Response.StatusCode = result.StatusCode;

                if (result.StatusCode == StatusCodes.Status404NotFound)
                    return;

                if (result.Payload != null)
                {
                    await context.Response.WriteAsJsonAsync(result.Payload, cancellationToken: context.RequestAborted).ConfigureAwait(false);
                    return;
                }

                await context.Response.WriteAsync("null", context.RequestAborted).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while processing query request.");

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred while processing the query.\"}", context.RequestAborted).ConfigureAwait(false);
                }
            }
        }
    }
}
