// Ignore Spelling: Keycloak

using Andor.Domain.Administrations.Languages;
using Andor.Domain.Engagement.Budget.Entities.Currencies;
using Andor.Domain.Onboarding.Users;
using System.Net.Mail;

namespace Andor.Application.Common.Interfaces;

public interface IKeycloakService
{
    Task<User> CreateUser(Guid Id,
    string UserName,
    string FirstName,
    string LastName,
    string Password,
    string Email,
    Guid Locale,
    Language Language,
    Currency Currency,
    bool AcceptedTermsCondition,
    bool AcceptedPrivateData,
    CancellationToken cancellationToken);

    Task<List<User>?> GetUserByEmail(MailAddress email, CancellationToken cancellation);
    Task<List<User>?> GetUserByUserName(string userName, CancellationToken cancellation);
    Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellation);
}
