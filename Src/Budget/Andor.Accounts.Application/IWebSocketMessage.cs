namespace Andor.Accounts.Application;

public interface IWebSocketMessage
{
    public Task SendAsync(Guid clientId, object message);
}
