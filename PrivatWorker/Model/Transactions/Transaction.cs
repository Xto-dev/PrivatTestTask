using System;
using System.Collections.Generic;
using System.Text;

namespace PrivatWorker.Model.Transactions
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public DateOnly Date { get; set; }               
        public decimal Amount { get; set; }       
        public int Status { get; set; }         
        public required TransactionMessage Message { get; set; }
    }

}
