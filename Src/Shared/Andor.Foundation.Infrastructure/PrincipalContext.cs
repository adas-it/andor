using Andor.Foundation.Application;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Foundation.Infrastructure;

public static class DbContextOptionsFactory
{
    public static DbContextOptions<TContext> Create<TContext>(string[] args)
        where TContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();

        //optionsBuilder.UseNpgsql("inmemory");

        return optionsBuilder.Options;
    }
}

public class PrincipalContext(DbContextOptions options, IMessageSenderInterface? messageSenderInterface)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Name>();
        modelBuilder.Ignore<Description>();
        modelBuilder.Ignore<Value>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.GetForeignKeys()
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                .ToList()
                .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
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

            foreach (var property in entityType.GetProperties()
                         .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
            {
                var converter = new ValueConverter<DateTime, DateTime>(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

                property.SetValueConverter(converter);
            }
        }
    }

    public void Upsert<T, TEntityId>(T entity)
        where T : Entity<TEntityId>
        where TEntityId : IEquatable<TEntityId>, IId<TEntityId>
    {
        if (Set<T>().Where(x => x.Id.Equals(entity.Id)).Any())
        {
            Set<T>().Update(entity);
        }
        else
        {
            Set<T>().Add(entity);
        }
    }

    public override int SaveChanges()
    {
        DispatchDomainEvents();

        return base.SaveChanges();
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        await DispatchDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void DispatchDomainEvents()
    {
        DispatchDomainEventsAsync(CancellationToken.None)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        if (messageSenderInterface == null)
        {
            return;
        }

        var domainEntities = this.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.Events != null && x.Entity.Events.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Events)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        foreach (var domainEvent in domainEvents)
        {
            await messageSenderInterface.PubSubSendAsync(domainEvent, cancellationToken);
        }

        return;
    }
}
