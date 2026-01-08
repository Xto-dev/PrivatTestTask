namespace PrivatWorker.Application.Transactions
{
    public interface ITransactionLogger
    {
        void TransactionCreated(Guid transactionId, int clientId);
        void TransactionsStatusChanged(bool parity, int affectedRows);
    }
}
