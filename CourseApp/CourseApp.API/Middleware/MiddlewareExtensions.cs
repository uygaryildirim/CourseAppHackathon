namespace CourseApp.API.Middleware;

// DÜZELTME: Middleware extension metodları eklendi. Middleware'leri pipeline'a eklemek için extension metodlar sağlanıyor.
public static class MiddlewareExtensions
{
    // DÜZELTME: Request logging middleware extension metodu. Pipeline'a request logging middleware'i ekleniyor.
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }

    // DÜZELTME: Global exception handler middleware extension metodu. Pipeline'a global exception handler middleware'i ekleniyor.
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}

