using System;
using System.Threading.Tasks;

namespace ClientServerComDef
{
    public interface IServerFunctions
    {
        int Ping(int id);
        void WriteMessage(string message);

        Task<LongProcessingTimeResp> Process(int secondsToSleep);
    }

    public class LongProcessingTimeResp
    {
        public long ProcessingTimeInMs { get; set; }
    }
}
