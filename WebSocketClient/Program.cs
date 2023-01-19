using ClientServerComDef;
using Eloe.InterfaceSerializer;
using System;

namespace WebSocketClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting");
            var logger = new Logger();
            var client = new WsClient(logger);
            var serverFunctions = client.AddServerInterface<IServerFunctions>();
            client.Connect("localhost", 9000);
            Console.WriteLine("connected");

            int pingNum = 1;
            while (true)
            {
                var m = Console.ReadLine();
                var res = serverFunctions.Ping(pingNum++);
                Console.WriteLine($"Ping result: {res}");
            }
        }
    }

    internal class Logger : ILogger
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
