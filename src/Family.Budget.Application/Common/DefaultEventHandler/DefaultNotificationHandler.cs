namespace Family.Budget.Application.@Common.DefaultEventHandler;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Models;
using Hangfire;
using MediatR;
using System.Threading.Tasks;
public class DefaultNotificationHandler : INotificationHandler<PublishIntegrationsEvents>
{
    public readonly IMessageSenderInterface message;

    public DefaultNotificationHandler(IMessageSenderInterface message)
    {
        this.message = message;
    }

    public Task Handle(PublishIntegrationsEvents notification, CancellationToken cancellationToken)
    {
        BackgroundJob.Enqueue(() => message.Send(notification.TopicName.Value, notification.Data));

        return Task.CompletedTask;
    }
}
