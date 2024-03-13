// Ignore Spelling: Keycloak

using Andor.Domain.Entities.Currencies;
using Andor.Domain.Entities.Languages;
using Andor.Domain.Entities.Users;
using System.Net.Mail;

namespace Andor.Application.Common.Interfaces;

public interface IKeycloakService
{
    Task<User> CreateUser(string Username,
        MailAddress Email,
        string FirstName,
        string LastName,
        string Password,
        Currency DefaultCurrency,
        Language DefaultLanguage,
        string? Avatar,
        CancellationToken cancellationToken);

    Task<List<User>?> GetUserByEmail(MailAddress email, CancellationToken cancellation);
    Task<List<User>?> GetUserByUserName(string username, CancellationToken cancellation);
    Task<User?> GetUserByUserId(Guid userId, CancellationToken cancellation);
}
