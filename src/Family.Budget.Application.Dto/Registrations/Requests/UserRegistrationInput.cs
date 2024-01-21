namespace Family.Budget.Application.Dto.Registrations.Registration;
public sealed record UserRegistrationInput(string FirstName,
        string LastName,
        string Email
    );

public sealed record RegistrationCheckEmail(string Email,
    string Code);

public sealed record CoachRegistrationResubmitEmailInput(string Email);