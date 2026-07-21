using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Andor.Foundation.Infrastructure.Outbox;
using Andor.Onboarding.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Onboarding.Binder.Outbox;

/// <summary>
/// Exposes one <see cref="OnboardingContext"/> per configured SQL Server tenant so the
/// generic <see cref="OutboxDispatcher"/> can drain each tenant's Outbox table.
/// </summary>
internal sealed class OnboardingOutboxContextProvider(ITenantService tenantService)
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

            var optionsBuilder = new DbContextOptionsBuilder<OnboardingContext>();
            _ = optionsBuilder.UseSqlServer(tenant.ConnectionString);

            contexts.Add(new OnboardingContext(optionsBuilder.Options));
        }

        return contexts;
    }
}
