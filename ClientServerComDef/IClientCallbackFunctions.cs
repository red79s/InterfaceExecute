namespace Eloe.ClientServerComDef
{
    public interface IClientCallbackFunctions
    {
        bool DispalayMessage(MessageInfo mi);
    }

    public class MessageInfo
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
