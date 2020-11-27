using EasyNetQ;
using MQ.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer
{
    public class RabbitMQBootstrapper
    {
        public static IBus CreateBus() => RabbitHutch.CreateBus("host=192.168.3.67;timeout=10");
    }
}
