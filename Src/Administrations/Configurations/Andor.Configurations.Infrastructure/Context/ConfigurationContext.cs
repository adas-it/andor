using System.Reflection;
using Andor.Configurations.Domain;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Andor.Configurations.Infrastructure.Context;

public class ConfigurationContextFactory : IDesignTimeDbContextFactory<ConfigurationContext>
{
    public ConfigurationContext CreateDbContext(string[] args)
    {
        var options = DbContextOptionsFactory.Create<ConfigurationContext>(args);
        return new ConfigurationContext(options);
    }
}

public class ConfigurationContext(DbContextOptions<ConfigurationContext> options)
    : PrincipalContext(options, null)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Configuration> Configuration => Set<Configuration>();
}
