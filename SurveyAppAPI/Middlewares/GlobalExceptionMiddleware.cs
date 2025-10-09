using DataAccessLayer.DTOs;
using System.Net;
using System.Text.Json;

namespace SurveyAppAPI.MiddleWares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Check if the request was cancelled
            if (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogWarning("Request was cancelled by the client");
                return; // Don't write response for cancelled requests
            }

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse();

            switch (exception)
            {
                case TaskCanceledException taskCancelled:
                    // Task was cancelled (could be timeout or cancellation)
                    _logger.LogInformation("Task was cancelled: {Message}", exception.Message);
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Message = taskCancelled.CancellationToken.IsCancellationRequested
                        ? "The request was cancelled"
                        : "The request timed out";
                    errorResponse.StatusCode = response.StatusCode;
                    break;

                case OperationCanceledException:
                    // General operation cancellation
                    _logger.LogInformation("Operation was cancelled: {Message}", exception.Message);
                    response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    errorResponse.Message = "The operation was cancelled";
                    errorResponse.StatusCode = response.StatusCode;
                    break;

                case UnauthorizedAccessException:
                    _logger.LogWarning(exception, "Unauthorized access attempt");
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = "Unauthorized access";
                    errorResponse.StatusCode = response.StatusCode;
                    break;

                case KeyNotFoundException:
                    _logger.LogWarning(exception, "Resource not found");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = "Resource not found";
                    errorResponse.StatusCode = response.StatusCode;
                    break;

                default:
                    // Unhandled errors
                    _logger.LogError(exception, "An unhandled exception occurred");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An internal server error occurred";
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Details = exception.Message; // Only in development
                    break;
            }

            try
            {
                // Check again before writing response
                if (!context.RequestAborted.IsCancellationRequested)
                {
                    var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    await response.WriteAsync(result, context.RequestAborted);
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected while we were writing the response
                _logger.LogInformation("Client disconnected before response could be written");
            }
        }
    }

}
