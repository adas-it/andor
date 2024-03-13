using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Domain.SeedWork;
using Family.Budget.Infrastructure.Repositories.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Family.Budget.Infrastructure.Repositories.Common;
public class UnitOfWork : IUnitOfWork
{
    private readonly PrincipalContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(PrincipalContext context,
            IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);

        await DispatchDomainEventsAsync();
    }
    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        _context.ChangeTracker.Entries()
        .Where(e => e.Entity != null).ToList()
        .ForEach(e => e.State = EntityState.Detached);

        return Task.CompletedTask;
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = _context.ChangeTracker
            .Entries<IAggregateRoot>()
            .Where(x => x.Entity.Events != null && x.Entity.Events.Any());

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.Events)
            .ToList();

        domainEntities.ToList()
            .ForEach(entity => entity.Entity.ClearEvents());

        foreach (var domainEvent in domainEvents.OrderBy(x => x.EventDateUtc))
            await _mediator.Publish(domainEvent);
    }
}