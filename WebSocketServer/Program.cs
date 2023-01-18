using WebSocketServer;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting ws server");
        var server = new WsServer("localhost", 9000);
        server.Start();
        Console.WriteLine("Server started");
        Console.ReadKey();
    }
}