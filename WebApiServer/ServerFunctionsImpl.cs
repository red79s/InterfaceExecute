using ClientServerComDef;
using Microsoft.AspNetCore.Authorization;

namespace WebApiServer
{
    public class ServerFunctionsImpl : IServerFunctions
    {
        public int Ping(int id)
        {
            return id + 1;
        }

        public Task<LongProcessingTimeResp> Process(int secondsToSleep)
        {
            return Task.FromResult<LongProcessingTimeResp>(new LongProcessingTimeResp { ProcessingTimeInMs = 10 });
        }

        [Authorize]
        public void WriteMessage(string message)
        {
            Console.WriteLine($"Received message: {message}");
        }
    }
}
