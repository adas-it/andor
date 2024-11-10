namespace Andor.Application.WebSocket
{
    public interface IWebSocketMessage
    {
        public Task SendAsync(Guid clientId, object message);
    }
}
