using System;
using System.Collections.Generic;
using System.Text;

namespace PrivatWorker.Infra.Logging
{
    public interface ILogWriter
    {
        void Write(string message, LogLevel level);
    }

    public enum LogLevel 
    {
        Info,
        Warning,
        Error 
    }

}
