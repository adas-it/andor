using Andor.Foundation.Application;
using Andor.Goals.RestApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Goals.Binder;

public static class GoalsExtensions
{
    public static WebApplicationBuilder UseGoals(this WebApplicationBuilder builder)
    {
        _ = builder.Services.AddScoped<ITenantService, TenantService>();

        _ = builder.Services.UseApi();

        return builder;
    }
}
