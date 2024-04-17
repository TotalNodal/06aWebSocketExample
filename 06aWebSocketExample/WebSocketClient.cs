using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class WebSocketClient
{
    public async Task Connect(string url)
    {
        ClientWebSocket socket = new ClientWebSocket();
        await socket.ConnectAsync(new Uri(url), CancellationToken.None);

        Console.WriteLine("Connected to server.");

        Task receiveTask = Receive(socket);
        Task sendTask = Send(socket, "client:");

        await Task.WhenAll(receiveTask, sendTask);
    }

    private async Task Receive(ClientWebSocket socket)
    {
        byte[] buffer = new byte[1024];
        while (socket.State == WebSocketState.Open)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult result = await socket.ReceiveAsync(segment, CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"server: {message}");
            }
        }
    }

    private async Task Send(ClientWebSocket socket, string prefix)
    {
        while (socket.State == WebSocketState.Open)
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

            await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}