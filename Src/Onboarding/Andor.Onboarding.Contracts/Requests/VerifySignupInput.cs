namespace Andor.Onboarding.Contracts.Requests;

public record VerifySignupInput(string Email, string Code, string Password, string PreferredLanguage, string? PreferredCurrency, OptIn OptIn);

public record OptIn(bool Marketing, bool TermsAndConditions, bool PrivacyPolicy);
