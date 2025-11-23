using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Avro.Notion.Api.Filters;

public sealed class HttpRequestExceptionFilter : IExceptionFilter
{
    private readonly ILogger<HttpRequestExceptionFilter> _logger;

    public HttpRequestExceptionFilter(ILogger<HttpRequestExceptionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case HttpRequestException httpException:
                HandleHttpRequestException(httpException, context);
                break;
            case InvalidOperationException invalidOperationException:
                HandleInvalidOperationException(invalidOperationException, context);
                break;
        }
    }

    private void HandleHttpRequestException(HttpRequestException exception, ExceptionContext context)
    {
        var statusCode = (int)(exception.StatusCode ?? HttpStatusCode.BadGateway);
        _logger.LogWarning(exception, "External HTTP request failed with status {StatusCode}", statusCode);

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = "External dependency error",
            Detail = exception.Message,
            Type = $"https://developer.mozilla.org/docs/Web/HTTP/Status/{statusCode}"
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
        context.ExceptionHandled = true;
    }

    private void HandleInvalidOperationException(InvalidOperationException exception, ExceptionContext context)
    {
        const int statusCode = StatusCodes.Status400BadRequest;
        _logger.LogError(exception, "Request failed due to invalid configuration");

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = "Configuration error",
            Detail = exception.Message,
            Type = $"https://developer.mozilla.org/docs/Web/HTTP/Status/{statusCode}"
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
        context.ExceptionHandled = true;
    }
}
