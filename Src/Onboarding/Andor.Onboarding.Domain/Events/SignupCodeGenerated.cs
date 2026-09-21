using Andor.Foundation.Domain.Events;

namespace Andor.Onboarding.Domain.Events;

public sealed record SignupCodeGenerated : DomainEvent
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Code { get; init; }
    public required string PreferredLanguage { get; init; }

    public static SignupCodeGenerated FromSignupRequest(SignupRequest request) => new()
    {
        EventName = nameof(SignupCodeGenerated),
        Id = request.Id.Value,
        Name = request.Name,
        Email = request.Email,
        Code = request.VerificationCode,
        PreferredLanguage = request.PreferredLanguage,
    };
}
