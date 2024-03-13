namespace Andor.Infrastructure.Services.Keycloak.Models;
public record CreateUser(bool Enabled,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    List<Credentials> Credentials,
    Attributes Attributes);

public record Credentials(string Type, string Value, bool Temporary);
public record Attributes(string[] Currency,
    string[] Language,
    string[] Avatar,
    string[] AcceptedTermsCondition,
    string[] AcceptedPrivateData);
