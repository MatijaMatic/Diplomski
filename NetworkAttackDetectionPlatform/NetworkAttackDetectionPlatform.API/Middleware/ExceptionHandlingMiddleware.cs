using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetworkAttackDetectionPlatform.API.Middleware
{
    internal sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            ProblemDetails problem;
            int statusCode;

            switch (exception)
            {
                case ArgumentNullException ane:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    problem = new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = ane.Message,
                        Status = statusCode
                    };
                    _logger.LogWarning(ane, "ArgumentNullException: {Message}", ane.Message);
                    break;
                case ArgumentOutOfRangeException aore:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    problem = new ProblemDetails
                    {
                        Title = "Validation error",
                        Detail = aore.Message,
                        Status = statusCode
                    };
                    _logger.LogWarning(aore, "Validation error: {Message}", aore.Message);
                    break;
                case ArgumentException ae:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    problem = new ProblemDetails
                    {
                        Title = "Validation error",
                        Detail = ae.Message,
                        Status = statusCode
                    };
                    _logger.LogWarning(ae, "Validation error: {Message}", ae.Message);
                    break;
                case InvalidOperationException ioe:
                    // Treat InvalidOperation as 400 — domain operations failing due to bad input/state
                    statusCode = (int)HttpStatusCode.BadRequest;
                    problem = new ProblemDetails
                    {
                        Title = "Invalid operation",
                        Detail = ioe.Message,
                        Status = statusCode
                    };
                    _logger.LogWarning(ioe, "Invalid operation: {Message}", ioe.Message);
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    problem = new ProblemDetails
                    {
                        Title = "An unexpected error occurred",
                        Detail = exception.Message,
                        Status = statusCode
                    };
                    _logger.LogError(exception, "Unhandled exception");
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(problem, options);
            await context.Response.WriteAsync(payload);
        }
    }
}
