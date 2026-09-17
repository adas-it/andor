namespace Andor.Onboarding.Service.Consumers;

public sealed class SignupVerifiedSubscriptionOptions
{
    public const string SectionName = "SignupVerifiedSubscription";

    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;
}
