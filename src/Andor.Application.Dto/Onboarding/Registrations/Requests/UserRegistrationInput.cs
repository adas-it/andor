namespace Andor.Application.Dto.Onboarding.Registrations.Requests;
public sealed record UserRegistrationInput(string FirstName,
        string LastName,
        string Email
    );

public sealed record RegistrationCheckEmail(string Email,
    string Code);

public sealed record RegistrationResubmitEmailInput(string Email);
