// Ignore Spelling: Upsert

using Andor.Domain.Administrations.Configurations;
using Andor.Domain.Administrations.Languages;
using Andor.Domain.Common;
using Andor.Domain.Communications;
using Andor.Domain.Communications.Users;
using Andor.Domain.Engagement.Budget.Accounts.Accounts;
using Andor.Domain.Engagement.Budget.Accounts.Categories;
using Andor.Domain.Engagement.Budget.Accounts.Currencies;
using Andor.Domain.Engagement.Budget.Accounts.Invites;
using Andor.Domain.Engagement.Budget.Accounts.PaymentMethods;
using Andor.Domain.Engagement.Budget.Accounts.SubCategories;
using Andor.Domain.Onboarding.Registrations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;

namespace Andor.Infrastructure.Repositories.Context;

public class PrincipalContextFactory : IDesignTimeDbContextFactory<PrincipalContext>
{
    public PrincipalContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PrincipalContext>();
        optionsBuilder.UseNpgsql("inmemory");

        return new PrincipalContext(optionsBuilder.Options);
    }
}

public partial class PrincipalContext(DbContextOptions<PrincipalContext> options) : DbContext(options)
{
    // Administration
    public DbSet<Configuration> Configuration => Set<Configuration>();
    public DbSet<Language> Language => Set<Language>();

    //Onboarding
    public DbSet<Registration> Registration => Set<Registration>();

    // Communication
    public DbSet<Template> Template => Set<Template>();
    public DbSet<Rule> Rule => Set<Rule>();
    public DbSet<Permission> Permission => Set<Permission>();
    public DbSet<Recipient> Recipient => Set<Recipient>();

    //Engagement
    public DbSet<SubCategory> SubCategory => Set<SubCategory>();
    public DbSet<PaymentMethod> PaymentMethod => Set<PaymentMethod>();
    public DbSet<Invite> Invite => Set<Invite>();
    public DbSet<Currency> Currency => Set<Currency>();
    public DbSet<Category> Category => Set<Category>();
    public DbSet<Account> Account => Set<Account>();
    public DbSet<AccountCategory> AccountCategory => Set<AccountCategory>();
    public DbSet<AccountSubCategory> AccountSubCategory => Set<AccountSubCategory>();
    public DbSet<AccountPaymentMethod> AccountPaymentMethod => Set<AccountPaymentMethod>();
    public DbSet<AccountUser> AccountUser => Set<AccountUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.GetForeignKeys()
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
        }

        foreach (var entityType in from entityType in modelBuilder.Model.GetEntityTypes()
                                   where typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType)
                                   select entityType)
        {
            modelBuilder.Entity(entityType.ClrType)
                            .Property<bool>(nameof(ISoftDeletableEntity.IsDeleted))
                            .HasDefaultValue(false);
            entityType.AddSoftDeleteQueryFilter();
        }

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public void Upsert<T>(T entity) where T : class
    {
        if (Entry(entity).State == EntityState.Detached)
            Set<T>().Add(entity);
    }

    public void UpsertRange<T>(IEnumerable<T> entities) where T : class
    {
        foreach (var entity in entities)
        {
            if (Entry(entity).State == EntityState.Detached)
            {
                Set<T>().Add(entity);
            }
        }
    }
}

public static class GlobalQueryExtensions
{
    public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityType)
    {
        var method = typeof(GlobalQueryExtensions)
            .GetMethod(nameof(GetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(entityType.ClrType);
        var filter = method.Invoke(null, []);
        entityType.SetQueryFilter((LambdaExpression)filter);
        entityType.AddIndex(entityType.FindProperty(nameof(ISoftDeletableEntity.IsDeleted)));
    }

    private static LambdaExpression GetSoftDeleteFilter<TEntity>()
        where TEntity : class, ISoftDeletableEntity
    {
        Expression<Func<TEntity, bool>> filter = x => !x.IsDeleted;
        return filter;
    }
}
