using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Foundation.ServerServices;

public static class CorsExtensions
{
    private const string PolicyName = "AllowConfiguredOrigins";

    public static void AddFoundationCors(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors").Get<Cors>()?.AllowedOrigins ?? [];

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy => policy
                .WithOrigins(allowedOrigins.ToArray())
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
    }

    public static void UseFoundationCors(this WebApplication app) => app.UseCors(PolicyName);
}
