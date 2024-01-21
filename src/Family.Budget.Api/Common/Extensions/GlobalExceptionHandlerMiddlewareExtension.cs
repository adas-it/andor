namespace Family.Budget.Api.Extensions;

using Family.Budget.Api.Middlewares;

public static class GlobalExceptionHandlerMiddlewareExtension
{
    public static IServiceCollection AddGlobalExceptionHandlerMiddleware(this IServiceCollection services)
    {
        return services.AddTransient<GlobalExceptionHandlerMiddleware>();
    }

    public static void UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
