using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.EasyNetQ
{
    // TODO
    public class ConnectionStringConfig
    {
        public string Host { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? RequestedHeartbeat { get; set; }
        public int? Prefetchcount { get; set; }
        public bool? PublisherConfirms { get; set; }
        public bool? PersistentMessages{ get; set; }
        public string Product { get; set; }
        public string Platform { get; set; }
        public int? Timeout { get; set; }
    }
}
