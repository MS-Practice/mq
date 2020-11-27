using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Account Account { get; set; }
        public string IdCard { get; set; }
    }
}
