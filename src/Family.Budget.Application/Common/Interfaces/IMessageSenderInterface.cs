namespace Family.Budget.Application.@Common.Interfaces;
public interface IMessageSenderInterface
{
    Task Send(string topicName, object data);
    Task SendQueue(string queueName, object data);
}
