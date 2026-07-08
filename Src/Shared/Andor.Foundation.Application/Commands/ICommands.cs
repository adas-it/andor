using Andor.Authorizations.Domain;

namespace Andor.Foundation.Application.Commands;

public interface ICommands<T>
{
    T Id { get; init; }

    ApplicationUser CurrentUser { get; }

    CancellationToken CancellationToken { get; init; }
}
