using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eloe.InteraceSerializerTests
{
    public class TestInterfaceImpl : ITestInterface
    {
        public bool InitCalled { get; set; }


        public void Init()
        {
            InitCalled = true;
        }

        public List<string> Items { get; set; } = new List<string>();
        public void SetItem(string item)
        {
            Items.Add(item);
        }

        public List<string> GetItems()
        {
            return Items;
        }

        public string GetItem(string itemKey)
        {
            if (Items.Contains(itemKey))
                return itemKey;
            return null;
        }

        public string GetItem(string itemKey, SomeReq req)
        {
            return "abc";
        }

        public Task SendMessage(string message)
        {
            return Task.CompletedTask;
        }

        public Task<bool> SendMessage2(string message, int num)
        {
            return Task<bool>.Run(() => true);
        }
    }
}