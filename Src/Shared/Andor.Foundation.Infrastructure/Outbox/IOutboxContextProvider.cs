namespace Andor.Foundation.Infrastructure.Outbox;

/// <summary>
/// Supplies the set of <see cref="PrincipalContext"/> instances whose Outbox tables
/// must be polled by the <see cref="OutboxDispatcher"/>. Each module implements this
/// to expose its own persistence sources (e.g. one context per tenant database),
/// keeping the dispatcher generic and reusable across modules.
/// </summary>
public interface IOutboxContextProvider
{
    /// <summary>
    /// Creates a fresh <see cref="PrincipalContext"/> for every Outbox source
    /// (typically one per tenant). Callers are responsible for disposing each context.
    /// </summary>
    IReadOnlyCollection<PrincipalContext> CreateContexts();
}
