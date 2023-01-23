using ClientServerComDef;
using Eloe.InterfaceSerializer;
using System;
using System.Diagnostics;

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

                    int processingTimeInSec = 0;
                    if (int.TryParse(m, out processingTimeInSec))
                    {
                        Console.WriteLine("Calling Process");
                        var sw = new Stopwatch();
                        sw.Start();
                        var processRes = serverFunctions.Process(processingTimeInSec);
                        sw.Stop();
                        Console.WriteLine($"Async Process returned in : {sw.ElapsedMilliseconds}ms");
                        sw.Start();
                        processRes.Wait();
                        sw.Stop();
                        Console.WriteLine($"Async Process finished in : {sw.ElapsedMilliseconds}ms, res: {processRes.Result.ProcessingTimeInMs}ms");
                    }
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
