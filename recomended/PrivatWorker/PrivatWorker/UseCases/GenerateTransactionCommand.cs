using PrivatWorker.Entities;

namespace PrivatWorker.UseCases;

public sealed class GenerateTransactionCommand(
    ITransactionLog log,
    ITransactionRepository repository,
    ITransactionServices services
) : ITransactionCommand
{
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var transaction = GenerateTransaction();
        await repository.AddTransactionAsync(transaction, cancellationToken);
        log.TransactionCreated(transaction);
    }

    private Transaction GenerateTransaction() => new()
    {
        Id = services.NewGuid(),
        Amount = (decimal)Math.Round(services.NextDouble() * 10000, 2),
        Date = services.Today(),
        Message = new TransactionMessage
        {
            AccountNumber = "UA" + services.NextInt(0, 1_000_000_000).ToString("D10"),
            ClientId = services.NextInt(1, 50001),
            OperationType = services.NextDouble() < 0.5 ? OperationType.offline : OperationType.online,
        }
    };
}