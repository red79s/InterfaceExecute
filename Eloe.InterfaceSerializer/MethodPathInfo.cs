namespace Eloe.InterfaceSerializer
{
    public class MethodPathInfo
    {
        public string MethodPath { get; set; }
        public bool HaveReturnValue { get; set; }
        public IInterfaceExecute Executer { get; set; }
    }
}