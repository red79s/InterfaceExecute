namespace Eloe.InterfaceSerializer.DataPacket
{
    public class FunctionDataPacket
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string FunctionName { get; set; }
        public string FunctionParameters { get; set; }
        public string JwtToken { get; set; }
    }
}
