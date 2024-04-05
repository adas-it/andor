// Ignore Spelling: Upsert

using Andor.Domain.Common;
using Andor.Domain.Entities.Admin.Configurations;
using Andor.Domain.Entities.Communications;
using Andor.Domain.Entities.Communications.Users;
using Andor.Domain.Entities.Onboarding.Registrations;
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
    public DbSet<Configuration> Configuration => Set<Configuration>();
    public DbSet<Registration> Registration => Set<Registration>();
    public DbSet<Template> Template => Set<Template>();
    public DbSet<Rule> Rule => Set<Rule>();
    public DbSet<Permission> Permission => Set<Permission>();
    public DbSet<Recipient> Recipient => Set<Recipient>();

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

            entityType.GetProperties()
                .Where(p => p.ClrType == typeof(string))
                .ToList()
                .ForEach(p => p.SetMaxLength(255));
        }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<bool>(nameof(ISoftDeletableEntity.IsDeleted))
                    .HasDefaultValue(false);
                entityType.AddSoftDeleteQueryFilter();
            }
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
        var filter = method.Invoke(null, Array.Empty<object>());
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
