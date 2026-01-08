
namespace PrivatWorker.Infra.Logging
{
    public class ErrorLogger
    {
        private readonly ILogWriter _logWriter;
        public ErrorLogger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public void LogError(string message, Exception ex)
        {
            string output = string.Format(
            "Message: {0}\nException: {1}",
            message,
            ex);

            _logWriter.Write(output, LogLevel.Error);
        }
    }
}
