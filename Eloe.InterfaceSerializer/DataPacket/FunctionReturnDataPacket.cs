namespace Eloe.InterfaceSerializer.DataPacket
{
    public class FunctionReturnDataPacket
    {
        public int Id { get; set; }
        public string ReturnValue { get; set; }
        public string Exception { get; set; }
    }
}
