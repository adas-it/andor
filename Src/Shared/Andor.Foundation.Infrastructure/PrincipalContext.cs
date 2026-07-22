using System.Text.Json;
using Andor.Foundation.Application;
using Andor.Foundation.Domain;
using Andor.Foundation.Domain.Events;
using Andor.Foundation.Domain.SeedWork;
using Andor.Foundation.Domain.ValuesObjects;
using Andor.Foundation.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Andor.Foundation.Infrastructure;

public static class DbContextOptionsFactory
{
    public static DbContextOptions<TContext> Create<TContext>(string[] args)
        where TContext : DbContext
    {
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();

        optionsBuilder.UseInMemoryDatabase("inmemory");

        return optionsBuilder.Options;
    }
}

public abstract class PrincipalContext(DbContextOptions options, IMessageSenderInterface? messageSenderInterface)
    : DbContext(options)
{
    /// <summary>
    /// Schema that owns this module's Outbox table. Each module must supply its own, since
    /// modules can share the same physical database.
    /// </summary>
    protected abstract string OutboxSchema { get; }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Name>();
        modelBuilder.Ignore<Description>();
        modelBuilder.Ignore<Value>();
        modelBuilder.Ignore<Andor.Foundation.Domain.Events.DomainEvent>();

        modelBuilder.ApplyConfiguration(new OutboxMessageConfig(OutboxSchema));

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
        EnqueueDomainEventsToOutbox();

        return base.SaveChanges();
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        EnqueueDomainEventsToOutbox();

        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Collects the domain events raised by tracked aggregates and persists them as
    /// <see cref="OutboxMessage"/> entries within the SAME change tracker, so they are
    /// committed atomically together with the aggregate changes (transactional Outbox).
    /// </summary>
    private void EnqueueDomainEventsToOutbox()
    {
        var domainEntities = this.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.Events != null && x.Entity.Events.Any())
            .ToList();

        if (domainEntities.Count == 0)
        {
            return;
        }

        var outboxMessages = domainEntities
            .SelectMany(x => x.Entity.Events)
            .Select(domainEvent =>
            {
                var eventType = domainEvent.GetType();

                return new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredOn = DateTimeOffset.UtcNow,
                    Type = eventType.AssemblyQualifiedName!,
                    Content = JsonSerializer.Serialize(domainEvent, eventType),
                    TargetQueue = (domainEvent as IQueueRoutedDomainEvent)?.QueueName,
                };
            })
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearEvents());

        Set<OutboxMessage>().AddRange(outboxMessages);
    }
}
