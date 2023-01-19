using System;

namespace ClientServerComDef
{
    public interface IServerFunctions
    {
        int Ping(int id);
        void WriteMessage(string message);
    }
}
