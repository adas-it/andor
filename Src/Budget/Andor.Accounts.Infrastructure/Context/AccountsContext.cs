using System.Reflection;
using Andor.Accounts.Domain.Accounts;
using Andor.Accounts.Domain.Currencies;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Andor.Accounts.Infrastructure.Context;

public class AccountsContextFactory : IDesignTimeDbContextFactory<AccountsContext>
{
    public AccountsContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccountsContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=andor_accounts;Trusted_Connection=True;");
        return new AccountsContext(optionsBuilder.Options);
    }
}

public class AccountsContext : PrincipalContext
{
    public AccountsContext(
        DbContextOptions<AccountsContext> options,
        IMessageSenderInterface? messageSenderInterface = null)
        : base(options, messageSenderInterface)
    {
    }

    protected override string OutboxSchema => "AccountsOutbox";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<Account> Account => Set<Account>();

    public DbSet<Currency> Currency => Set<Currency>();
}
