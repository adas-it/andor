using Andor.Communications.Binder.Application;
using Andor.Communications.Binder.Infrastructure;
using Andor.Communications.RestApi;
using Andor.Foundation.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Communications.Binder;

public static class CommunicationExtensions
{
    public static WebApplicationBuilder UseCommunications(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        _ = builder.Services.AddScoped<ITenantService, TenantService>();

        _ = builder.Services.UseApi()
            .WithCommunicationApplication()
            .WithCommunicationInfrastructure(configuration);

        return builder;
    }

    public static Task ApplyCommunicationMigrationsAsync(this WebApplication app)
        => app.Services.ApplyCommunicationMigrationsAsync();
}
