namespace CourseApp.API.Middleware;

// DÜZELTME: Middleware extension metodları eklendi. Middleware'leri pipeline'a eklemek için extension metodlar sağlanıyor.
public static class MiddlewareExtensions
{
    // DÜZELTME: Request logging middleware extension metodu. Pipeline'a request logging middleware'i ekleniyor.
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }

    // DÜZELTME: Exception handling middleware extension metodu. Pipeline'a exception handling middleware'i ekleniyor.
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

