namespace Family.Budget.Kernel.Extensions;

using Family.Budget.Application.Models.Config;
using Family.Budget.Infrastructure.Services.Keycloak;
using Family.Budget.Infrastructure.Services.Keycloak.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Refit;
using System;

public static class HttpClientsExtension
{
    public static IServiceCollection AddHttpClientsAsync(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<KeycloackAuthenticationHandler>();

        string keyCloackUrl = configuration["Keycloack:Url"]!;

        services
            .AddRefitClient<IKeycloackClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(keyCloackUrl))
            .AddHttpMessageHandler<KeycloackAuthenticationHandler>()
            .AddPolicyHandler(GetRetryPolicy(configuration))
            .AddPolicyHandler(GetCircuitBreakerPolicy(configuration));

        return services;
    }

    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IConfiguration configuration)
    {
        var wrc = new PollyConfigs(
            configuration[$"{PollyConfigs.PollyConfig}:{nameof(PollyConfigs.Repetitions)}"]!,
            configuration[$"{PollyConfigs.PollyConfig}:{nameof(PollyConfigs.TimeCircuitBreak)}"]!,
            configuration[$"{PollyConfigs.PollyConfig}:{nameof(PollyConfigs.TimeOut)}"]!
            );

        Random jitterer = new();

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(int.Parse(wrc.Repetitions),
                _ => TimeSpan.FromMilliseconds(int.Parse(wrc.TimeCircuitBreak)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IConfiguration configuration)
    {
        var wrc = new PollyConfigs(
            configuration[$"{PollyConfigs.PollyConfig}:{nameof(PollyConfigs.Repetitions)}"]!,
            configuration[$"{PollyConfigs.PollyConfig}:{nameof(PollyConfigs.TimeCircuitBreak)}"]!,
            configuration[$"{PollyConfigs.PollyConfig}:{nameof(PollyConfigs.TimeOut)}"]!
            );

        return Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(int.Parse(wrc.TimeOut)));
    }
}
