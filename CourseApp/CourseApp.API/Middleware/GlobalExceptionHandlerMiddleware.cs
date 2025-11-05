using CourseApp.ServiceLayer.Utilities.Result;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace CourseApp.API.Middleware;

// DÜZELTME: Global exception handler middleware eklendi. Tüm exception'lar yakalanıp standart bir formatta döndürülüyor, API response tutarlılığı sağlanıyor.
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // DÜZELTME: Exception loglanıyor. Hata ayıklama ve izleme için exception detayları, stack trace ve request bilgileri kaydediliyor.
            _logger.LogError(ex, 
                "An unhandled exception occurred. Method: {Method}, Path: {Path}, Message: {Message}", 
                context.Request.Method, 
                context.Request.Path, 
                ex.Message);
            
            await HandleExceptionAsync(context, ex);
        }
    }

    // DÜZELTME: Global exception handling metodu. Tüm exception'lar standart bir API response formatına dönüştürülüyor, exception tipine göre özel mesajlar döndürülüyor.
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, message, errors) = GetExceptionDetails(exception);
        
        context.Response.StatusCode = (int)statusCode;

        // DÜZELTME: Exception response modeli oluşturuluyor. Development ortamında stack trace bilgisi de ekleniyor.
        var response = new
        {
            isSuccess = false,
            message = message,
            errors = errors,
            // DÜZELTME: Development ortamında stack trace gösteriliyor, production'da güvenlik için gösterilmiyor.
            stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
        };

        // DÜZELTME: Exception response'u JSON formatında döndürülüyor. API response tutarlılığı sağlanıyor.
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    // DÜZELTME: Exception tipine göre HTTP status code, mesaj ve hata detayları belirleniyor. Farklı exception tipleri için özel mesajlar sağlanıyor.
    private (HttpStatusCode statusCode, string message, List<string>? errors) GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            // DÜZELTME: FluentValidation exception'ları için özel handling. Validation hataları kullanıcıya detaylı olarak gösteriliyor.
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "Girdi doğrulama hatası oluştu.",
                validationException.Errors.Select(e => e.ErrorMessage).ToList()
            ),
            
            // DÜZELTME: ArgumentNullException için özel mesaj. Null parametre hatası için açıklayıcı mesaj döndürülüyor.
            ArgumentNullException argNullEx => (
                HttpStatusCode.BadRequest,
                $"Eksik parametre hatası: {argNullEx.ParamName ?? "Bilinmeyen parametre"} boş olamaz.",
                null
            ),
            
            // DÜZELTME: ArgumentException için özel mesaj. Geçersiz parametre hatası için açıklayıcı mesaj döndürülüyor.
            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                $"Geçersiz parametre: {argEx.Message}",
                null
            ),
            
            // DÜZELTME: KeyNotFoundException için özel mesaj. Kayıt bulunamadığında 404 döndürülüyor.
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                "İstenen kayıt bulunamadı.",
                null
            ),
            
            // DÜZELTME: UnauthorizedAccessException için özel mesaj. Yetkilendirme hatası için 401 döndürülüyor.
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "Bu işlem için yetkiniz bulunmamaktadır.",
                null
            ),
            
            // DÜZELTME: DbUpdateException için özel handling. Veritabanı güncelleme hataları için açıklayıcı mesaj döndürülüyor.
            DbUpdateException dbEx => (
                HttpStatusCode.BadRequest,
                "Veritabanı işlemi sırasında bir hata oluştu. Lütfen verilerinizi kontrol edin.",
                _environment.IsDevelopment() ? new List<string> { dbEx.InnerException?.Message ?? dbEx.Message } : null
            ),
            
            // DÜZELTME: DbUpdateConcurrencyException için özel handling. Eşzamanlılık hataları için açıklayıcı mesaj döndürülüyor.
            DbUpdateConcurrencyException => (
                HttpStatusCode.Conflict,
                "Kayıt başka bir kullanıcı tarafından güncellenmiş. Lütfen tekrar deneyin.",
                null
            ),
            
            // DÜZELTME: NotImplementedException için özel mesaj. Henüz implement edilmemiş metodlar için açıklayıcı mesaj döndürülüyor.
            NotImplementedException => (
                HttpStatusCode.NotImplemented,
                "Bu özellik henüz implement edilmemiştir.",
                null
            ),
            
            // DÜZELTME: TimeoutException için özel mesaj. Zaman aşımı hataları için açıklayıcı mesaj döndürülüyor.
            TimeoutException => (
                HttpStatusCode.RequestTimeout,
                "İstek zaman aşımına uğradı. Lütfen tekrar deneyin.",
                null
            ),
            
            // DÜZELTME: Diğer tüm exception'lar için genel mesaj. Production'da detaylı hata mesajı gösterilmiyor, güvenlik için.
            _ => (
                HttpStatusCode.InternalServerError,
                _environment.IsDevelopment() 
                    ? $"Bir hata oluştu: {exception.Message}" 
                    : "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.",
                null
            )
        };
    }
}

