using Andor.Communications.Infrastructure.Context;
using Andor.Foundation.Infrastructure;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Andor.Communications.Binder.Outbox;

/// <summary>
/// Exposes the single <see cref="CommunicationContext"/> so the generic
/// <see cref="OutboxDispatcher"/> can drain its Outbox table.
/// </summary>
internal sealed class CommunicationOutboxContextProvider(IConfiguration configuration)
    : IOutboxContextProvider
{
    public IReadOnlyCollection<PrincipalContext> CreateContexts()
    {
        var optionsBuilder = new DbContextOptionsBuilder<CommunicationContext>();
        _ = optionsBuilder.UseSqlServer(configuration.GetConnectionString("Communication"));

        return [new CommunicationContext(optionsBuilder.Options)];
    }
}
