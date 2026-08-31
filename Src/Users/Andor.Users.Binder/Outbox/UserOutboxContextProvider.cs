using Andor.Users.Infrastructure.Context;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Andor.Users.Binder.Outbox;

/// <summary>
/// Exposes one <see cref="UserContext"/> per configured SQL Server tenant so the generic
/// <see cref="OutboxDispatcher"/> can drain each tenant's Outbox table.
/// </summary>
internal sealed class UserOutboxContextProvider(ITenantService tenantService)
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

            var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
            _ = optionsBuilder.UseSqlServer(tenant.ConnectionString);

            contexts.Add(new UserContext(optionsBuilder.Options));
        }

        return contexts;
    }
}
