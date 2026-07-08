namespace Andor.Foundation.Application;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, CancellationToken cancellationToken);
}
