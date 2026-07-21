using System.Reflection;
using Andor.Configurations.Domain;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Andor.Configurations.Infrastructure.Context;

public class ConfigurationContextFactory : IDesignTimeDbContextFactory<ConfigurationContext>
{
    public ConfigurationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=andor_configurations;Trusted_Connection=True;");
        return new ConfigurationContext(optionsBuilder.Options);
    }
}

public class ConfigurationContext : PrincipalContext
{
    public ConfigurationContext(
        DbContextOptions<ConfigurationContext> options,
        IMessageSenderInterface? messageSenderInterface = null)
        : base(options, messageSenderInterface)
    {
    }

    protected override string OutboxSchema => "ConfigurationsOutbox";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Configuration> Configuration => Set<Configuration>();
}
