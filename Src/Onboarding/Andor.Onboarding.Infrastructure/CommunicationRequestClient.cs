using System.Net.Http.Headers;
using System.Net.Http.Json;
using Andor.Communications.Contracts.Requests;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Onboarding.Application.Interfaces;
using Andor.Onboarding.Domain.Errors;

namespace Andor.Onboarding.Infrastructure;

public sealed class CommunicationRequestClient(HttpClient httpClient, IUsersIdentityTokenProvider tokenProvider)
    : ICommunicationRequestClient
{
    public async Task<DomainResult> RequestAsync(
        Guid ruleId,
        string templateTitle,
        Guid? userId,
        string? recipientEmail,
        string? contentLanguage,
        Dictionary<string, string>? values,
        CancellationToken cancellationToken)
    {
        var input = new RequestCommunicationInput(ruleId, templateTitle, userId, recipientEmail, contentLanguage, values);

        var token = await tokenProvider.GetAccessTokenAsync(cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/communications/requests")
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
                nameof(ICommunicationRequestClient),
                $"Communications.Service returned {(int)response.StatusCode} while requesting rule '{ruleId}'.",
                SignupErrorCodes.CommunicationRequestFailed),
        });
    }
}
