using Andor.Foundation.Domain.Events;

namespace Andor.Onboarding.Domain.Events;

public sealed record SignupCodeGenerated : DomainEvent, IQueueRoutedDomainEvent
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Code { get; init; }

    public string QueueName => "signup-verification-codes";

    public static SignupCodeGenerated FromSignupRequest(SignupRequest request) => new()
    {
        Id = request.Id.Value,
        Name = request.Name,
        Email = request.Email,
        Code = request.VerificationCode,
    };
}
