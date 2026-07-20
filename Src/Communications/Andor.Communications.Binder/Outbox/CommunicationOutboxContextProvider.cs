using Andor.Communications.Infrastructure.Context;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Andor.Communications.Binder.Outbox;

/// <summary>
/// Exposes one <see cref="CommunicationContext"/> per configured SQL Server tenant so the
/// generic <see cref="OutboxDispatcher"/> can drain each tenant's Outbox table.
/// </summary>
internal sealed class CommunicationOutboxContextProvider(ITenantService tenantService)
    : IOutboxContextProvider
{
    public IReadOnlyCollection<PrincipalContext> CreateContexts()
    {
        var contexts = new List<PrincipalContext>();

        foreach (var tenant in tenantService.GetTenants())
        {
            if (tenant.DatabaseType != TenantService.SQLServer)
            {
                continue;
            }

            var optionsBuilder = new DbContextOptionsBuilder<CommunicationContext>();
            _ = optionsBuilder.UseSqlServer(tenant.ConnectionString);

            contexts.Add(new CommunicationContext(optionsBuilder.Options));
        }

        return contexts;
    }
}
