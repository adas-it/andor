using Andor.Application.Common.Interfaces;
using Andor.Domain.SeedWork;
using Andor.Infrastructure.Repositories.Context;
using Microsoft.EntityFrameworkCore;

namespace Andor.Infrastructure.Repositories.Common;

public class UnitOfWork(PrincipalContext context, IMessageSenderInterface messageSenderInterface) : IUnitOfWork
{
    private readonly PrincipalContext _context = context;
    private readonly IMessageSenderInterface _messageSenderInterface = messageSenderInterface;

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await DispatchDomainEventsAsync(cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        _context.ChangeTracker.Entries()
        .Where(e => e.Entity != null).ToList()
        .ForEach(e => e.State = EntityState.Detached);

        return Task.CompletedTask;
    }

    private Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = _context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.Events != null && x.Entity.Events.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Events)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        foreach (var domainEvent in domainEvents.OrderBy(x => x.EventDateUTC))
        {
            _messageSenderInterface.PubSubSendAsync(domainEvent, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
