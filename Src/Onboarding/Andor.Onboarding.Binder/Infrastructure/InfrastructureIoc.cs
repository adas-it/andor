using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Andor.Onboarding.Binder.Outbox;
using Andor.Onboarding.Domain.Repositories;
using Andor.Onboarding.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Andor.Onboarding.Binder.Infrastructure;

internal static class InfrastructureIoc
{
    internal static IServiceCollection WithOnboardingInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.WithOnboardingDbContext(configuration);

        services.WithAzureServiceBusMessaging(configuration);

        services.AddScoped<ICommandsSignupRequestRepository, CommandsSignupRequestRepository>();

        services.AddScoped<IOutboxContextProvider, OnboardingOutboxContextProvider>();

        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
