using Andor.Foundation.Binder;
using Andor.Onboarding.Application;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Onboarding.Binder.Application;

internal static class ApplicationIoc
{
    public static IServiceCollection WithOnboardingApplication(this IServiceCollection services)
    {
        services.AddSingleton<IAkkaModule, ApplicationAkkaModule>();

        services.AddScoped<IOnboardingValidator, OnboardingValidator>();

        services.AddScoped<ISignupCommandsService, SignupCommandsService>();

        return services;
    }
}
