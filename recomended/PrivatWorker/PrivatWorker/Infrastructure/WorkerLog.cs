namespace PrivatWorker.Infrastructure;

public sealed class WorkerLog(
    ILogger<WorkerLog> logger
) : IWorkerLog
{
    public void ExecuteAsyncException(Exception exception) => logger.LogError(exception, "Execute async failed.");
}