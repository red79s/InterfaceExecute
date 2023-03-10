
using ClientServerComDef;
using Eloe.InterfaceSerializer;
using System.Diagnostics;
using WebSocketServer;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting ws server");
        var logger = new Logger();
        var server = new WsServer("localhost", 9000, logger);
        server.ImplementInterface<IServerFunctions>(new ServerInterfaceImpl());
        server.Start();
        Console.WriteLine("Server started");

        int counter = 1;
        while (true)
        {
            Console.ReadKey();
            var clients = server.GetConnectedClients();
            foreach (var client in clients)
            {
                var clientCallback = server.AddClientCallbackInterface<IClientCallbackFunctions>(client.ClientId);
                clientCallback.DispalayMessage(new MessageInfo { Message = $"Message from server: {counter++}", Name = "server" });
            }
        }
    }

    private class ServerInterfaceImpl : IServerFunctions
    {
        public int Ping(int id)
        {
            //if (id % 4 == 0)
            //    throw new Exception("Invalid number");
            return id + 1;
        }

        public async Task<LongProcessingTimeResp> Process(int secondsToSleep)
        {
            var sw = new Stopwatch();
            sw.Start();
            await Task.Delay(secondsToSleep * 1000);
            sw.Stop();
            return new LongProcessingTimeResp { ProcessingTimeInMs = sw.ElapsedMilliseconds };
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine("In WriteMessage: " + message);
        }
    }

    private class Logger : ILogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string message)
        {
            Console.WriteLine(message);
        }

        public void Fatal(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void Warn(string message)
        {
            Console.WriteLine(message);
        }
    }
}