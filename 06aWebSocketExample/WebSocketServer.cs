using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketServer
{
    public async Task Start()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.WriteLine("Server started. Listening on http://localhost:8080/");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                await ProcessWebSocketRequest(context, "server:");
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private async Task ProcessWebSocketRequest(HttpListenerContext context, string prefix)
    {
        HttpListenerWebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);
        WebSocket socket = webSocketContext.WebSocket;

        while (socket.State == WebSocketState.Open)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                Console.WriteLine($"{prefix} Received message: {message}");

                // Echo the received message back to the client
                ArraySegment<byte> outputBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"{prefix} Echo: {message}"));
                await socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
            }
        }
    }
}
