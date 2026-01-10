namespace PrivatWorker.UseCases;

public class UpdateTransactionStatusCommand(
    ITransactionLog log,
    ITransactionRepository repository,
    ITransactionServices services
) : ITransactionCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var nowSecond = services.NowSecond();
        var isParity = nowSecond % 2 == 0;
        var rows = await repository.ChangeTransactionsStatusByParityAsync(isParity, cancellationToken);
        log.TransactionsStatusChanged(isParity, rows);
    }
}