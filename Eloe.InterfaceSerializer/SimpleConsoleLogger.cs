using System;
using System.Collections.Generic;
using System.Text;

namespace Eloe.InterfaceSerializer
{
    public class SimpleConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            WriteToConsole("Debug", message);
        }

        public void Error(string message)
        {
            WriteToConsole("Error", message);
        }

        public void Fatal(string message)
        {
            WriteToConsole("Fatal", message);
        }

        public void Info(string message)
        {
            WriteToConsole("Info", message);
        }

        public void Trace(string message)
        {
            WriteToConsole("Trace", message);
        }

        public void Warn(string message)
        {
            WriteToConsole("Warn", message);
        }

        public void WriteToConsole(string logLevel, string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")}[{logLevel}] {message}");
        }
    }
}
