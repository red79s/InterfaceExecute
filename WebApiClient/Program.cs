using ClientServerComDef;
using Eloe.InterfaceRpc;
using System.Text.Json;
using WebApiClent;

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
            var httpClient = new HttpClient();

            //Get token using client_credentials flow
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string,string>("grant_type","client_credentials"),
                    new KeyValuePair<string, string>("client_id",clientId),
                    new KeyValuePair<string, string>("client_secret",clientSecret),
                    new KeyValuePair<string, string>("tenant_id", "AMBE"),
                    new KeyValuePair<string, string>("scope","api offline_access") // this will get a refresh token in addition to the regular access token, if you dont want that just skip adding "offline_access" to scope
                });

            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            Console.WriteLine($"Getting token from: {tokenUrl}");
            var authRes = httpClient.PostAsync(tokenUrl, content).Result;
            Console.WriteLine($"Response: {authRes.StatusCode}");
            string resp = authRes.Content.ReadAsStringAsync().Result;

            //Get token from response
            var tokenResp = JsonSerializer.Deserialize<TokenResponse>(resp);
            if (tokenResp == null)
            {
                throw new Exception("Unable to get token");
            }
            Console.WriteLine($"Token: {tokenResp.access_token.Substring(0, 50)}...");

            var client = new InterfaceRpcSendCollectionHttpClient("https://localhost:7010", tokenResp.access_token);
            var serverInterface = client.AddProxyCallbackInterface<IServerFunctions>();

            var client2 = new InterfaceRpcSendCollectionHttpClient("https://localhost:7010", "");
            var serverInterface2 = client2.AddProxyCallbackInterface<IServerFunctions>();

            var client3 = new InterfaceRpcSendCollectionHttpClient("https://localhost:7010", "");
            var serverInterface3 = client3.AddProxyCallbackInterface<IServerFunctions>();

            int pingVal = 1;
            while (true)
            {
                var line = Console.ReadLine();
                serverInterface.WriteMessage(line);
                var res = serverInterface2.Ping(pingVal);
                Console.WriteLine($"ping response: {res}");
                pingVal++;

                try
                {
                    serverInterface3.WriteMessage("line");
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