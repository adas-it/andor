namespace Andor.Users.Service.Consumers;

public sealed class UserVerifiedSubscriptionOptions
{
    public const string SectionName = "UserVerifiedSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}
