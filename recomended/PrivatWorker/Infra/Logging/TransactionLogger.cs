using PrivatWorker.Application.Transactions;

namespace PrivatWorker.Infra.Logging
{
    public class TransactionLogger : ITransactionLogger
    {
        private readonly ILogWriter _logWriter;
        public TransactionLogger(ILogWriter logWriter)
        {
            _logWriter = logWriter;
        }
        public void TransactionCreated(Guid transactionId, int clientId)
        {
            string output = string.Format("Inserted operation {0}", transactionId);
            _logWriter.Write(output, LogLevel.Info);
        }

        public void TransactionsStatusChanged(bool isParity, int affectedRows)
        {
            string output = string.Format("Updated {0} rows. Parity is {1}", affectedRows, isParity);
            _logWriter.Write(output, LogLevel.Info);
        }
    }
}
