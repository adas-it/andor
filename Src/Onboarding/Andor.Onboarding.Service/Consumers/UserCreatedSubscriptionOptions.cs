namespace Andor.Onboarding.Service.Consumers;

public sealed class UserCreatedSubscriptionOptions
{
    public const string SectionName = "UserCreatedSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}
