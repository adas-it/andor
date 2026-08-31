using Andor.Foundation.Domain.ValuesObjects;
using Andor.Users.Domain.Users.ValueObjects;

namespace Andor.Users.Application.Commands;

public record CreateUserCommand(
    UserId Id,
    Email Email,
    string FirstName,
    string LastName,
    Guid PreferredCurrencyId,
    Guid PreferredLanguageId,
    CancellationToken CancellationToken);
