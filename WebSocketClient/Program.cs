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
            client.ImplementInterface<IClientCallbackFunctions>(new ClientCallbackImpl());
            client.Connect("localhost", 9000);
            Console.WriteLine("connected");

            int pingNum = 1;
            while (true)
            {
                var m = Console.ReadLine();
                try
                {
                    var res = serverFunctions.Ping(pingNum++);
                    Console.WriteLine($"Ping result: {res}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
               
                serverFunctions.WriteMessage(m);
            }
        }
    }

    internal class ClientCallbackImpl : IClientCallbackFunctions
    {
        public bool DispalayMessage(MessageInfo mi)
        {
            Console.WriteLine($"Received callback: {mi.Name}, {mi.Message}");
            return true;
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
