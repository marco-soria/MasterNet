using MasterNet.Application.Core;
using Newtonsoft.Json;

namespace MasterNet.WebApi.Middleware;

public class ExceptionMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env
    )
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var response = ex switch
            {
                ValidationException validationException => new AppException(
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    string.Join("; ", validationException.Errors.Select(er => er.ErrorMessage))
                ),

                _ => new AppException(
                    StatusCodes.Status500InternalServerError,
                    _env.IsDevelopment() ? ex.Message : "An error occurred while processing your request",
                    _env.IsDevelopment() ? ex.StackTrace?.ToString() : null
                )
            };

            // ✅ LOGGING MEJORADO: Solo loggear errores 500+ como Error, 400 como Warning
            if (response.StatusCode >= 500)
            {
                _logger.LogError(ex, "Server error occurred: {Message}", ex.Message);
            }
            else if (response.StatusCode >= 400)
            {
                _logger.LogWarning("Client error occurred: {StatusCode} - {Message}", response.StatusCode, response.Message);
            }

            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = "application/json";
            var json = JsonConvert.SerializeObject(response);
            await context.Response.WriteAsync(json);
        }
    }
}