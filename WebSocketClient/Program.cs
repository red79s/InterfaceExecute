using System;

namespace WebSocketClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting");
            var client = new WsClient("localhost", 9000);
            client.Connect();
            Console.WriteLine("connected");

            while (true)
            {
                var m = Console.ReadLine();
                client.SendMessage(m);
            }
        }
    }
}
