using ClientServerComDef;
using Eloe.InterfaceRpc;
using WebApiClient;

internal class Program
{
    private static void Main(string[] args)
    {
        var clientId = "test";
        var clientSecret = "Test10";

        Console.WriteLine("Starting WebApiClient");
        
        try
        {
            var tokenUrl = "https://auth.apx-systems.com/connect/token";
            var tokenManager = new TokenManager();
            var token = tokenManager.GetToken(tokenUrl, clientId, clientSecret);

            var client = new InterfaceRpcSendCollectionHttpClient("https://localhost:7010", token.access_token);
            var serverInterface = client.AddProxyCallbackInterface<IServerFunctions>();

            int pingVal = 1;
            while (true)
            {
                try
                {
                    var line = Console.ReadLine();
                    serverInterface.WriteMessage(line);
                    var res = serverInterface.Ping(pingVal);
                    Console.WriteLine($"ping response: {res}");
                    pingVal++;
                }
                catch ( Exception ex )
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled exception: {ex}");
        }

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
}