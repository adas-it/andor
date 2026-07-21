namespace Andor.Users.WebApi.Consumers;

/// <summary>
/// Standalone Service Bus config for this API's one consumer. Kept local (rather than reusing
/// Andor.Foundation.Infrastructure's sender-oriented ServiceBusOptions/ServiceBusIoc) since this
/// is the only service in the system that receives messages instead of just publishing them, and
/// it intentionally doesn't take a dependency on Foundation.Infrastructure otherwise.
/// </summary>
public sealed class UserVerifiedSubscriptionOptions
{
    public const string SectionName = "ServiceBus";

    public string? FullyQualifiedNamespace { get; set; }
    public string? ConnectionString { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}
