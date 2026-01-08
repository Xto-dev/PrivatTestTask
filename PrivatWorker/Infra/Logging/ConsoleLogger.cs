using System;
using System.Collections.Generic;
using System.Text;

namespace PrivatWorker.Infra.Logging
{
    public class ConsoleLogWriter : ILogWriter
    {
        public void Write(string message, LogLevel level)
        {
            Console.WriteLine($"[{level}] {message}");
        }
    }
}
