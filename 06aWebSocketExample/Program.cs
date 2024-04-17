using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06aWebSocketExample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            WebSocketServer server = new WebSocketServer();
            WebSocketClient client = new WebSocketClient();

            var serverTask = server.Start();
            var clientTask = client.Connect("ws://localhost:8080/");

            await Task.WhenAll(serverTask, clientTask);
        }
    }

    public static class WebSocketServerExtensions
    {
        public static async Task Start(this WebSocketServer server)
        {
            await server.StartInternal();
        }

        private static async Task StartInternal(this WebSocketServer server)
        {
            await server.Start();
        }
    }

    public static class WebSocketClientExtensions
    {
        public static async Task Connect(this WebSocketClient client, string url)
        {
            await client.ConnectInternal(url);
        }

        private static async Task ConnectInternal(this WebSocketClient client, string url)
        {
            await client.Connect(url);
        }
    }
}

