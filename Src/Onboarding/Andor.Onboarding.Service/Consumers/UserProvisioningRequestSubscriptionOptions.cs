namespace Andor.Onboarding.Service.Consumers;

public sealed class UserProvisioningRequestSubscriptionOptions
{
    public const string SectionName = "UserProvisioningRequestSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}
