namespace Family.Budget.Infrastructure.Gateway.Keycloak.Models;
public record CreateUser(bool Enabled, 
	string Username, 
	string Email, 
	string FirstName, 
	string LastName,
    List<Credentials> Credentials,
    Attributes Attributes);

public record Credentials(string Type, string Value, bool Temporary);
public record Attributes(string[] Locale, 
	string[] Avatar,
	string[] AcceptedTermsCondition,
	string[] AcceptedPrivateData);

