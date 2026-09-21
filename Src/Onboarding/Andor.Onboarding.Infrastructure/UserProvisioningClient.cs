using System.Net.Http.Headers;
using System.Net.Http.Json;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Domain.Errors;
using Andor.Users.Contracts.Requests;

namespace Andor.Onboarding.Infrastructure;

public sealed class UserProvisioningClient(HttpClient httpClient, IUsersIdentityTokenProvider tokenProvider)
    : IUserProvisioningClient
{
    public async Task<DomainResult> ProvisionAsync(
        Guid userId,
        string name,
        string email,
        string passwordHash,
        bool marketingOptIn,
        bool termsAndConditionsAccepted,
        bool privacyPolicyAccepted,
        CancellationToken cancellationToken)
    {
        var (firstName, lastName) = SplitName(name);

        var input = new CreateUserInput(
            userId, email, firstName, lastName, passwordHash,
            marketingOptIn, termsAndConditionsAccepted, privacyPolicyAccepted);

        var token = await tokenProvider.GetAccessTokenAsync(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/users")
        {
            Content = JsonContent.Create(input),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return DomainResult.Success();
        }

        return DomainResult.Failure(errors: new[]
        {
            new Notification(
                nameof(IUserProvisioningClient),
                $"Users.Service returned {(int)response.StatusCode} while provisioning user '{userId}'.",
                SignupErrorCodes.ProvisioningFailed),
        });
    }

    private static (string FirstName, string LastName) SplitName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (name, name);
        }

        var parts = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        return parts.Length == 2 ? (parts[0], parts[1]) : (parts[0], parts[0]);
    }
}
