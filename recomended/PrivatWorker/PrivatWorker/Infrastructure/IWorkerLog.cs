namespace PrivatWorker.Infrastructure;
public interface IWorkerLog
{
    void ExecuteAsyncException(Exception exception);
}