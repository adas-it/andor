using System.Reflection;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Andor.Users.Domain.Users;

namespace Andor.Users.Infrastructure.Context;

public class UserContextFactory : IDesignTimeDbContextFactory<UserContext>
{
    public UserContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=andor_users;Trusted_Connection=True;");
        return new UserContext(optionsBuilder.Options);
    }
}

public class UserContext : PrincipalContext
{
    public UserContext(
        DbContextOptions<UserContext> options,
        IMessageSenderInterface? messageSenderInterface = null)
        : base(options, messageSenderInterface)
    {
    }

    protected override string OutboxSchema => "UsersOutbox";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<User> Users => Set<User>();
}
