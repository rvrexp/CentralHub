using CentralHub.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace CentralHub.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Try to let the request go through the rest of the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // If an exception occurs, catch it and handle it
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set the response content type
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new
            {
                Success = false,
                Message = "An error occurred." // Default message
            };

            // Customize the response based on the exception type
            switch (exception)
            {
                case NotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                    errorResponse = new { Success = false, Message = ex.Message };
                    break;

                // Add this case later when we implement the validation pipeline
                // case ValidationException ex:
                //     response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                //     errorResponse = new { Success = false, Errors = ex.Errors };
                //     break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                    errorResponse = new { Success = false, Message = "An internal server error has occurred." };
                    _logger.LogError(exception, "An unhandled exception occurred."); // Log the full error
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
