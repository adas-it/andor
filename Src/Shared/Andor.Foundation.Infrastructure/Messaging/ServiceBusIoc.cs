using Andor.Foundation.Application;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Andor.Foundation.Infrastructure.Messaging;

/// <summary>
/// Dependency injection extensions to register the Azure Service Bus topic publisher.
/// </summary>
public static class ServiceBusIoc
{
    /// <summary>
    /// Registers <see cref="IMessageSenderInterface"/> backed by an Azure Service Bus topic sender.
    /// Uses <see cref="DefaultAzureCredential"/> (Managed Identity) whenever a fully qualified
    /// namespace is configured; falls back to a connection string only if provided.
    /// </summary>
    public static IServiceCollection WithAzureServiceBusMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ServiceBusOptions>()
            .Bind(configuration.GetSection(ServiceBusOptions.SectionName))
            .Validate(options =>
                    !string.IsNullOrWhiteSpace(options.TopicName) &&
                    (!string.IsNullOrWhiteSpace(options.FullyQualifiedNamespace) ||
                     !string.IsNullOrWhiteSpace(options.ConnectionString)),
                "ServiceBus configuration requires a TopicName and either a FullyQualifiedNamespace (recommended) or a ConnectionString.")
            .ValidateOnStart();

        // A single ServiceBusClient should be shared across the application (it manages
        // the underlying connections and is thread-safe).
        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;

            var clientOptions = new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
                RetryOptions = new ServiceBusRetryOptions
                {
                    Mode = ServiceBusRetryMode.Exponential,
                    MaxRetries = 5,
                    Delay = TimeSpan.FromMilliseconds(500),
                    MaxDelay = TimeSpan.FromSeconds(30),
                },
            };

            // An explicit connection string takes precedence when provided (e.g. local dev
            // via User Secrets). Otherwise use Managed Identity via DefaultAzureCredential,
            // which is the recommended, secret-less approach for Azure environments.
            if (!string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                return new ServiceBusClient(options.ConnectionString, clientOptions);
            }

            var environment = serviceProvider.GetService<IHostEnvironment>();

            var credentialOptions = new DefaultAzureCredentialOptions();

            // Locally there is no IMDS endpoint (169.254.169.254), so the ManagedIdentity
            // probe hangs and fails. Skip it during development and rely on the developer's
            // Visual Studio / Azure CLI sign-in instead.
            if (environment?.IsDevelopment() == true)
            {
                credentialOptions.ExcludeManagedIdentityCredential = true;
                credentialOptions.ExcludeWorkloadIdentityCredential = true;
            }

            return new ServiceBusClient(
                options.FullyQualifiedNamespace,
                new DefaultAzureCredential(credentialOptions),
                clientOptions);
        });

        services.AddSingleton(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
            var client = serviceProvider.GetRequiredService<ServiceBusClient>();

            return client.CreateSender(options.TopicName);
        });

        services.AddScoped<IMessageSenderInterface, MessageSenderAzure>();

        return services;
    }
}
