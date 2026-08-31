namespace Andor.Users.Contracts.Responses;

public record UserPreferencesOutput(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    Guid PreferredCurrencyId,
    Guid PreferredLanguageId);
