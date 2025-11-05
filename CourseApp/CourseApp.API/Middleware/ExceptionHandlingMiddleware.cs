using CourseApp.ServiceLayer.Utilities.Result;
using System.Net;
using System.Text.Json;

namespace CourseApp.API.Middleware;

// DÜZELTME: Exception handling middleware eklendi. Tüm exception'lar yakalanıp standart bir formatta döndürülüyor, API response tutarlılığı sağlanıyor.
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
            // DÜZELTME: Exception loglanıyor. Hata ayıklama ve izleme için exception detayları kaydediliyor.
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    // DÜZELTME: Exception handling metodu. Tüm exception'lar standart bir API response formatına dönüştürülüyor.
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ErrorResult("Bir hata oluştu. Lütfen daha sonra tekrar deneyin.");
        
        // DÜZELTME: Exception tipine göre HTTP status code belirleniyor. Farklı exception tipleri için uygun status code'lar kullanılıyor.
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        // DÜZELTME: Exception response'u JSON formatında döndürülüyor. API response tutarlılığı sağlanıyor.
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

