using System.Net;
using System.Text.Json;

namespace LabMS.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                message = "An error occurred while processing your request",
                details = GetExceptionDetails(exception),
                timestamp = DateTime.UtcNow,
                traceId = context.TraceIdentifier
            }
        };

        switch (exception)
        {
            case ArgumentNullException:
            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = "Invalid request parameters",
                        details = exception.Message,
                        timestamp = DateTime.UtcNow,
                        traceId = context.TraceIdentifier
                    }
                };
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    error = new
                    {
                        message = "Unauthorized access",
                        details = "You do not have permission to access this resource",
                        timestamp = DateTime.UtcNow,
                        traceId = context.TraceIdentifier
                    }
                };
                break;

            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    error = new
                    {
                        message = "Invalid operation",
                        details = exception.Message,
                        timestamp = DateTime.UtcNow,
                        traceId = context.TraceIdentifier
                    }
                };
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    error = new
                    {
                        message = "Resource not found",
                        details = exception.Message,
                        timestamp = DateTime.UtcNow,
                        traceId = context.TraceIdentifier
                    }
                };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static string GetExceptionDetails(Exception exception)
    {
        // In development, return full exception details
        // In production, return generic message
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        if (environment == "Development")
        {
            return $"{exception.Message}\n{exception.StackTrace}";
        }
        
        return "An unexpected error occurred. Please try again later.";
    }
}
