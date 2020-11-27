using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Shared.Messages
{
    public class TransferInAccountEvent
    {
        public TransferInAccountEvent(decimal price, string userName, string accountNumber)
        {
            Price = price;
            UserName = userName;
            AccountNumber = accountNumber;
        }

        public decimal Price { get; }
        public string UserName { get; }
        public string AccountNumber { get; }
    }
}
