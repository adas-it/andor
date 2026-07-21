using Andor.Foundation.Infrastructure;
using Andor.Onboarding.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Onboarding.Binder.Infrastructure;

internal static class InfrastructureDbContext
{
    internal static IServiceCollection WithOnboardingDbContext(this IServiceCollection services,
        IConfiguration configuration)
        => services.WithTenantDbContext<OnboardingContext>();

    internal static Task ApplyOnboardingMigrationsAsync(this IServiceProvider serviceProvider)
        => serviceProvider.ApplyTenantMigrationsAsync<OnboardingContext>(
            options => new OnboardingContext(options));
}
