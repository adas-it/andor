using Andor.Application.WebSocket;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Andor.Api.WebSocketTest
{
    public class WebSocketMessages : IWebSocketMessage
    {
        public record WebSocketConnection(Guid id, Guid sessionId, WebSocket socket);

        public static List<WebSocketConnection> WebSocketConnections = new();
        public async Task SendAsync(Guid clientId, object message)
        {

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var collection = WebSocketConnections.Where(x => x.id == clientId).ToList();
            if (collection.Any() is false) { return; }

            var messageBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, options));

            foreach (var item in collection)
            {
                if (item.socket.State == WebSocketState.Open)
                {
                    await item.socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
