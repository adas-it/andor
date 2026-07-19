namespace Andor.Foundation.Application;

public interface IMessageSenderInterface
{
    Task PubSubSendAsync(object data, string messageId, CancellationToken cancellationToken);
}
