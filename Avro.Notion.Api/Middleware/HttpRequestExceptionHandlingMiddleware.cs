using System.Net;
using System.Text.Json;

namespace Avro.Notion.Api.Middleware;

public sealed class HttpRequestExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<HttpRequestExceptionHandlingMiddleware> _logger;

    public HttpRequestExceptionHandlingMiddleware(RequestDelegate next, ILogger<HttpRequestExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            var statusCode = (int)(ex.StatusCode ?? HttpStatusCode.BadGateway);
            _logger.LogError(ex, "External HTTP request failed with status {StatusCode}", statusCode);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = "https://developer.mozilla.org/docs/Web/HTTP/Status/" + statusCode,
                title = "External dependency error",
                detail = ex.Message,
                status = statusCode,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem)).ConfigureAwait(false);
        }
    }
}
