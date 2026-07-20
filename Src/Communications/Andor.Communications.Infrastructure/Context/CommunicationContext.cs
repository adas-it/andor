using System.Reflection;
using Andor.Communications.Domain;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Andor.Communications.Infrastructure.Context;

public class CommunicationContextFactory : IDesignTimeDbContextFactory<CommunicationContext>
{
    public CommunicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CommunicationContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=andor_communications;Trusted_Connection=True;");
        return new CommunicationContext(optionsBuilder.Options);
    }
}

public class CommunicationContext : PrincipalContext
{
    public CommunicationContext(
        DbContextOptions<CommunicationContext> options,
        IMessageSenderInterface? messageSenderInterface = null)
        : base(options, messageSenderInterface)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Rule> Rule => Set<Rule>();

    public DbSet<Template> Template => Set<Template>();
}
