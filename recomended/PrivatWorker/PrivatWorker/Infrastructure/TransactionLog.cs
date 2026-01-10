using PrivatWorker.Entities;
using PrivatWorker.UseCases;

namespace PrivatWorker.Infrastructure;

public class TransactionLog(
    ILogger<TransactionLog> logger
) : ITransactionLog
{
    public void ExecuteAsyncException(Exception exception)
    {
        logger.LogError(exception, "Execute async failed.");
    }

    public void TransactionCreated(Transaction transaction)
    {
        logger.LogInformation($"Transaction created. Transaction id: {transaction.Id}");
    }

    public void TransactionsStatusChanged(bool isParity, int rows)
    {
        logger.LogInformation($"Transaction status changed. isParity: {isParity}. rows: {rows}");
    }
}