namespace Andor.Application.Common.Interfaces;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, CancellationToken cancellationToken);
}
