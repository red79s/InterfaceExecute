using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eloe.InteraceSerializerTests
{
    public interface ITestInterface
    {
        void Init();
        void SetItem(string item);
        List<string> GetItems();
        string GetItem(string itemKey);
        string GetItem(string itemKey, SomeReq req);
        Task SendMessage(string message);
        Task<bool> SendMessage2(string message, int num);
    }

    public class SomeReq
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}