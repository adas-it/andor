namespace Andor.Onboarding.Contracts.Requests;

public record VerifySignupInput(string Email, string Code, string Password);
