namespace PrivatWorker.UseCases;

public interface ITransactionCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}