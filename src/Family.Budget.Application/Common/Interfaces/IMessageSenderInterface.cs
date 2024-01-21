namespace Family.Budget.Application.@Common.Interfaces;
using Family.Budget.Application.Dto.Models.Events;

public interface IMessageSenderInterface
{
    Task Send(string topicName, object data);
    Task SendQueue(string queueName, object data);
}
