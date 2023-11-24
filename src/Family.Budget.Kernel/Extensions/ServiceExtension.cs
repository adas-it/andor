namespace Family.Budget.Kernel.Extensions;

using Family.Budget.Infrastructure.Services.FeatureFlag;
using Microsoft.Extensions.DependencyInjection;
using Family.Budget.Application.Models.Authorization;
using Family.Budget.Infrastructure.Services.Keycloak;
using Family.Budget.Infrastructure.rabbitmq;
using Family.Budget.Infrastructure.Services.Communications.RequestComunication;
using Family.Budget.Application.Common.Interfaces;

public static class ServicesExtension
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IFeatureFlagService, FeatureFlagService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddTransient<IKeycloackService, KeycloackService>();
        services.AddTransient<IMessageSenderInterface, SendMessageRabbitmq>();
        services.AddTransient<IRequestRegistrationComunication, RequestRegistrationComunication>();

        return services;
    }
}