using CentralHub.Application.Exceptions;
using FluentValidation;
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
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            // Use a Dictionary for a flexible response object
            var errorResponse = new Dictionary<string, object>
            {
                ["Success"] = false,
                ["Message"] = "An error occurred."
            };

            // Customize the response based on the exception type
            switch (exception)
            {
                case NotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                    errorResponse["Message"] = ex.Message;
                    break;

                case ValidationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                    var validationErrors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                    errorResponse["Message"] = "Validation failed.";
                    errorResponse["Errors"] = validationErrors; // Add the 'Errors' key
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                    errorResponse["Message"] = "An internal server error has occurred.";
                    _logger.LogError(exception, "An unhandled exception occurred.");
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
