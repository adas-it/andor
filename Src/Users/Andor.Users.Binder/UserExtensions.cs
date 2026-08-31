using Andor.Users.Binder.Application;
using Andor.Users.Binder.Infrastructure;
using Andor.Users.RestApi;
using Andor.Foundation.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Users.Binder;

public static class UserExtensions
{
    public static WebApplicationBuilder UseUsers(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        _ = builder.Services.AddScoped<ITenantService, TenantService>();

        _ = builder.Services.UseApi()
            .WithUserApplication()
            .WithUserInfrastructure(configuration);

        return builder;
    }

    public static Task ApplyUserMigrationsAsync(this WebApplication app)
        => app.Services.ApplyUserMigrationsAsync();
}
