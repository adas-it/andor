namespace Family.Budget.ComponentTest.Utils;

using Family.Budget.Application.Common.Interfaces;
using Family.Budget.Application.Dto.Models.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TopicMessageTestHelper : IMessageSenderInterface
{
    public List<EventList> list;

    public TopicMessageTestHelper()
    {
        list = new List<EventList>();
    }
    public Task Send(string topicName, object data)
    {
        list.Add(new EventList(topicName, (IntegrationEvent)data));

        return Task.CompletedTask;
    }

    public Task SendQueue(string queueName, object data)
    {
        list.Add(new EventList(queueName, (IntegrationEvent)data));

        return Task.CompletedTask;
    }
}

public class EventList
{
    public EventList(string name, IntegrationEvent data)
    {
        Name = name;
        Data = data;
    }

    public string Name { get; set; }

    public IntegrationEvent Data { get; set; }
}
