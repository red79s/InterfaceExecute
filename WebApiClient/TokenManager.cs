using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eloe.WebApiClient
{
    internal class TokenManager
    {
        public TokenResponse GetToken(string url, string clientId, string clientSecret)
        {
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

            Console.WriteLine($"Getting token from: {url}");
            var authRes = httpClient.PostAsync(url, content).Result;
            Console.WriteLine($"Response: {authRes.StatusCode}");
            string resp = authRes.Content.ReadAsStringAsync().Result;

            //Get token from response
            var tokenResp = JsonSerializer.Deserialize<TokenResponse>(resp);
            if (tokenResp == null)
            {
                throw new Exception("Unable to get token");
            }
            Console.WriteLine($"Token: {tokenResp.access_token.Substring(0, 50)}...");

            return tokenResp;
        }
    }
}
