namespace Family.Budget.Infrastructure.rabbitmq;

using Family.Budget.Application.Common;
using Family.Budget.Application.Common.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

public class SendMessageRabbitmq : IMessageSenderInterface
{
    public SendMessageRabbitmq(IOptions<ApplicationSettings> appConfig)
    {
        hostName = appConfig.Value.Rabbitmq?.HostName;
        hostPort = appConfig.Value.Rabbitmq?.HostPort;
        virtualHost = appConfig.Value.Rabbitmq?.VirtualHost;
        username = appConfig.Value.Rabbitmq?.Username;
        password = appConfig.Value.Rabbitmq?.Password;
    }

    private readonly string? hostName;
    private readonly string? hostPort;
    private readonly string? virtualHost;
    private readonly string? username;
    private readonly string? password;

    public Task Send(string name, object data)
    {
        IModel model = Connect();

        var properties = model.CreateBasicProperties();

        properties.Persistent = false;

        byte[] messagebuffer = Encoding.Default.GetBytes(JsonConvert.SerializeObject(data));

        model.BasicPublish(name, "", properties, messagebuffer);

        return Task.CompletedTask;
    }

    public Task SendQueue(string queueName, object data)
    {
        IModel model = Connect();

        var properties = model.CreateBasicProperties();

        properties.Persistent = false;

        byte[] messagebuffer = Encoding.Default.GetBytes(JsonConvert.SerializeObject(data));

        model.BasicPublish("", queueName, properties, messagebuffer);

        return Task.CompletedTask;
    }

    private IModel Connect()
    {
        var connectionFactory = new ConnectionFactory();

        if (!string.IsNullOrEmpty(hostName))
            connectionFactory.HostName = hostName;

        if (!string.IsNullOrEmpty(hostPort))
        {
            var port = int.Parse(hostPort);
            connectionFactory.Port = port;
        }
        else
        {
            connectionFactory.Port = AmqpTcpEndpoint.UseDefaultPort;
        }

        if (!string.IsNullOrEmpty(virtualHost))
            connectionFactory.VirtualHost = virtualHost;

        if (!string.IsNullOrEmpty(username))
            connectionFactory.UserName = username;

        if (!string.IsNullOrEmpty(password))
            connectionFactory.Password = password;

        connectionFactory.DispatchConsumersAsync = true;

        var connection = connectionFactory.CreateConnection();

        var model = connection.CreateModel();
        return model;
    }
}
