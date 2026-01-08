using System;
using System.Collections.Generic;
using System.Text;

namespace PrivatWorker.Model.Transactions
{
    public class TransactionMessage
    {
        public int ClientId { get; set; }
        public string AccountNumber { get; set; } = "";
        public required OperationType Type { get; set; }
    }
}
