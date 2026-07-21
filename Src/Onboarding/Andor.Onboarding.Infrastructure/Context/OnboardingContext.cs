using System.Reflection;
using Andor.Foundation.Application;
using Andor.Foundation.Infrastructure;
using Andor.Onboarding.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Andor.Onboarding.Infrastructure.Context;

public class OnboardingContextFactory : IDesignTimeDbContextFactory<OnboardingContext>
{
    public OnboardingContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OnboardingContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=andor_onboarding;Trusted_Connection=True;");
        return new OnboardingContext(optionsBuilder.Options);
    }
}

public class OnboardingContext : PrincipalContext
{
    public OnboardingContext(
        DbContextOptions<OnboardingContext> options,
        IMessageSenderInterface? messageSenderInterface = null)
        : base(options, messageSenderInterface)
    {
    }

    protected override string OutboxSchema => "OnboardingOutbox";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _ = modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<SignupRequest> SignupRequest => Set<SignupRequest>();
}
