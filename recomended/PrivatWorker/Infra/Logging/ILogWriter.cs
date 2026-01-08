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
