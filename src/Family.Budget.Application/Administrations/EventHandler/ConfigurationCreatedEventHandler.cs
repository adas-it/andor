namespace Family.Budget.Application.Administrations.EventHandler;

using Family.Budget.Domain.Entities.Admin.DomainEvents;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ConfigurationCreatedEventHandler : INotificationHandler<ConfigurationCreatedDomainEvent>
{
    public Task Handle(ConfigurationCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("Foi");

        return Task.CompletedTask;
    }
}
