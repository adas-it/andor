using Microsoft.Extensions.Configuration;

namespace Andor.Foundation.Application;

public class TenantService : ITenantService
{
    public static readonly string SQLServer = "SQLServer";

    public List<Tenant> Tenants { get; }

    public TenantService(IConfiguration configuration)
    {
        Tenants = new List<Tenant>();

        configuration.GetRequiredSection("Tenants").Bind(Tenants);
    }

    public List<Tenant> GetTenants()
    {
        return Tenants;
    }
}
