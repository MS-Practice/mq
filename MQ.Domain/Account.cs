using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.Domain
{
    public class Account
    {
        public int Id { get; set; }
        public string BankNumber { get; set; }
        public string IdCard { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset UpdationTime { get; set; }

        public void Transferout(decimal money)
        {
            if (money > Price)
                throw new InvalidOperationException("money not enough!");
            Price -= money;
        }

        public void TransferIn(decimal money)
        {
            Price = checked(Price + money);
        }
    }
}
