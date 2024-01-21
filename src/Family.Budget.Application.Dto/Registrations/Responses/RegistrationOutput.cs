namespace Family.Budget.Application.Dto.Registrations.Responses;

public record RegistrationOutput(string FirstName,
        string LastName,
        string Email,
        string? Language = "en"
    );
