using System.Reflection;
using Andor.Authorizations.Domain;
using Andor.Configurations.Domain;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Andor.Configurations.Infrastructure.Context;

public class ConfigurationContextFactory : IDesignTimeDbContextFactory<ConfigurationContext>
{
    public ConfigurationContext CreateDbContext(string[] args)
    {
        var options = DbContextOptionsFactory.Create<ConfigurationContext>(args);
        return new ConfigurationContext(options);
    }
}

public class ConfigurationContext
    : PrincipalContext
{
    private readonly ICurrentUserService? currentUserService;
    private readonly ITenantService? tenantService;
    private readonly IConfiguration? configuration;

    public ConfigurationContext(
        DbContextOptions<ConfigurationContext> options)
        : this(options, null, null, null, null)
    {
    }

    public ConfigurationContext(
        DbContextOptions<ConfigurationContext> options,
        ICurrentUserService? currentUserService,
        IConfiguration? configuration,
        ITenantService? tenantService,
        IMessageSenderInterface? messageSenderInterface)
        : base(options, messageSenderInterface)
    {
        this.currentUserService = currentUserService;
        this.configuration = configuration;
        this.tenantService = tenantService;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (currentUserService is null || configuration is null)
        {
            base.OnConfiguring(optionsBuilder);
            return;
        }

        var tenantName = currentUserService.GetCurrentUser().Tenant;

        var tenant = tenantService?.GetTenants().FirstOrDefault(x => x.Name.Contains(tenantName));

        if (tenant?.DatabaseType == TenantService.SQLServer)
        {

#if DEBUG
            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
#endif

            optionsBuilder.UseSqlServer(tenant.ConnectionString);

        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Configuration> Configuration => Set<Configuration>();
}
