namespace Andor.Application.Dto.Onboarding.Registrations.Responses;

public record RegistrationOutput(string FirstName,
        string LastName,
        string Email,
        string? Language = "en"
    );
