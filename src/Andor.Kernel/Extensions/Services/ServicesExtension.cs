using Andor.Application.Common;
using Andor.Application.Common.Interfaces;
using Andor.Infrastructure.Services.Keycloak;
using Andor.Infrastructure.Services.Keycloak.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Refit;

namespace Andor.Kernel.Extensions.Services;

public static class ServicesExtension
{
    public static WebApplicationBuilder AddServicesExtensionServices(this WebApplicationBuilder builder)
    {
        var configs = builder.Configuration
            .GetSection(nameof(Keycloak))
            .Get<Keycloak>();

        var pllyConfigs = builder.Configuration
            .GetSection(nameof(PollyConfigs))
            .Get<PollyConfigs>();

        builder.Services
            .AddRefitClient<IKeycloakAuthClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(configs.Url))
            .AddPolicyHandler(GetRetryPolicy(pllyConfigs))
            .AddPolicyHandler(GetCircuitBreakerPolicy(pllyConfigs));

        builder.Services.AddTransient<KeycloakAuthenticationHandler>();

        builder.Services
            .AddRefitClient<IKeycloakClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(configs.Url))
            .AddHttpMessageHandler<KeycloakAuthenticationHandler>()
            .AddPolicyHandler(GetRetryPolicy(pllyConfigs))
            .AddPolicyHandler(GetCircuitBreakerPolicy(pllyConfigs));

        builder.Services.AddTransient<IKeycloakService, KeycloakService>();

        return builder;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(PollyConfigs configuration)
    {
        Random jitterer = new();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(int.Parse(configuration.Repetitions),
                _ => TimeSpan.FromMilliseconds(int.Parse(configuration.TimeCircuitBreak)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(PollyConfigs configuration)
    {
        return Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(int.Parse(configuration.TimeOut)));
    }
}