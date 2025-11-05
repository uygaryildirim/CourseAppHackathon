using System.Diagnostics;

namespace CourseApp.API.Middleware;

// DÜZELTME: Request logging middleware eklendi. Tüm HTTP istekleri loglanıyor, API kullanımını izlemek ve debug için faydalı.
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // DÜZELTME: İstek başlangıç zamanı kaydediliyor. Response time hesaplaması için kullanılıyor.
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        // DÜZELTME: Gelen istek loglanıyor. HTTP method, path ve timestamp bilgileri kaydediliyor.
        _logger.LogInformation("Incoming Request: {Method} {Path} at {Time}", 
            requestMethod, requestPath, DateTime.UtcNow);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            // DÜZELTME: İstek tamamlandıktan sonra response time loglanıyor. Performans izleme için kullanılıyor.
            _logger.LogInformation("Completed Request: {Method} {Path} - Status: {StatusCode} - Duration: {ElapsedMilliseconds}ms",
                requestMethod, requestPath, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}

