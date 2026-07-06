using System.Linq.Expressions;
using System.Reflection;
using Andor.Assets.Domain.Investments.Movements;
using Andor.Assets.Domain.Investments.Tickers;
using Andor.Foundation.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Andor.Assets.Infrastructure;

public class InvestmentContextFactory : IDesignTimeDbContextFactory<InvestmentContext>
{
    public InvestmentContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InvestmentContext>();

        return new InvestmentContext(optionsBuilder.Options);
    }
}

public partial class InvestmentContext(DbContextOptions<InvestmentContext> options) : DbContext(options)
{
    // Administration
    public DbSet<Ticker> Tickers => Set<Ticker>();
    public DbSet<Movement> InvestmentMovements => Set<Movement>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

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
