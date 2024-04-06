namespace Andor.Infrastructure.Onboarding.Services.Keycloak.Models;
public record CreateUser(
    string FirstName,
    string LastName,
    string Email,
    bool Enabled,
    bool EmailVerified,
    string Username,
    List<Credentials> Credentials,
    Attributes Attributes,
    string[] Groups);

public record Credentials(string Type, string Value, bool Temporary);
public record Attributes(
    string[] Avatar,
    string[] AcceptedTermsCondition,
    string[] AcceptedPrivateData);
