using System.Net;
using System.Text.Json;
using Pudra.Connect.Application.Dtos.Common;

namespace Pudra.Connect.Api.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            // Hatanın detaylarını logluyoruz. Bu, bizim için çok önemli.
            _logger.LogError(ex, "An unhandled exception has occurred.");

            // İstemciye (mobil uygulama) standart bir hata cevabı hazırlıyoruz.
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ErrorResponseDto
            {
                StatusCode = context.Response.StatusCode,
                Message = "An internal server error has occurred. Please try again later."
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}