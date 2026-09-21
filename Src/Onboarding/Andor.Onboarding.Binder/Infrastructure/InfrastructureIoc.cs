using Andor.Foundation.Infrastructure.Messaging;
using Andor.Foundation.Infrastructure.Outbox;
using Andor.Onboarding.Application.Interfaces;
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

        services.AddOptions<UsersIdentityClientOptions>()
            .Bind(configuration.GetSection(UsersIdentityClientOptions.SectionName));

        services.AddHttpClient<IUsersIdentityTokenProvider, UsersIdentityTokenProvider>(client =>
        {
            var authority = configuration["IdentityProvider:Authority"] ?? "https://localhost:7116";
            client.BaseAddress = new Uri(authority.TrimEnd('/') + "/");
        });

        services.AddHttpClient<IUserProvisioningClient, UserProvisioningClient>(client =>
        {
            // "https+http://user-service" resolves via Aspire service discovery locally; set
            // UsersServiceClient:BaseAddress to override outside the AppHost (e.g. production).
            var baseAddress = configuration["UsersServiceClient:BaseAddress"] ?? "https+http://user-service";
            client.BaseAddress = new Uri(baseAddress);
        });

        return services;
    }
}
