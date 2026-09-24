using System.Net.WebSockets;
using static Andor.Accounts.Service.WebSockets.WebSocketMessages;

namespace Andor.Accounts.Service.WebSockets;

public static class Class
{
    internal static async Task EchoWebSocket(Guid clientId, Guid sessionId, WebSocket webSocket, List<WebSocketConnection> webSocketConnections)
    {
        WebSocketReceiveResult result = null;
        try
        {

            var buffer = new byte[1024 * 4];
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            webSocketConnections.Where(x => x.id == clientId && x.sessionId == sessionId).ToList().ForEach(x => webSocketConnections.Remove(x));
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        catch (Exception)
        {
            if (result != null)
            {
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        }
    }
}
