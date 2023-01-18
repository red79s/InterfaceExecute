using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SerialMessages
{
    public class FunctionReturnTaskCompletion<T> : TaskCompletionSource<T>
    {
        public string Id { get; set; }
    }
}
