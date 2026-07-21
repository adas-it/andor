using Andor.Foundation.Application;
using Andor.Onboarding.Binder.Application;
using Andor.Onboarding.Binder.Infrastructure;
using Andor.Onboarding.RestApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Onboarding.Binder;

public static class OnboardingExtensions
{
    public static WebApplicationBuilder UseOnboarding(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        _ = builder.Services.AddScoped<ITenantService, TenantService>();

        _ = builder.Services.UseApi()
            .WithOnboardingApplication()
            .WithOnboardingInfrastructure(configuration);

        return builder;
    }

    public static Task ApplyOnboardingMigrationsAsync(this WebApplication app)
        => app.Services.ApplyOnboardingMigrationsAsync();
}
