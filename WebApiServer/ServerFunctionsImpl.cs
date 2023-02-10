using ClientServerComDef;

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

        public void WriteMessage(string message)
        {
            
        }
    }
}
