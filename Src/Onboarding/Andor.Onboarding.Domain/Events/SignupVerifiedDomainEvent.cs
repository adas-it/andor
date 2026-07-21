using Andor.Foundation.Domain.Events;

namespace Andor.Onboarding.Domain.Events;

public sealed record SignupVerifiedDomainEvent : DomainEvent
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }

    public static SignupVerifiedDomainEvent FromSignupRequest(SignupRequest request, Guid userId, string passwordHash) => new()
    {
        Id = request.Id.Value,
        UserId = userId,
        Name = request.Name,
        Email = request.Email,
        PasswordHash = passwordHash,
    };
}
