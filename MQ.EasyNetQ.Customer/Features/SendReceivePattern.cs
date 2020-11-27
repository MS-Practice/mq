using EasyNetQ;
using MQ.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer.Features
{
    public class SendReceivePattern
    {
        public static void Start()
        {
            using (var bus = RabbitMQBootstrapper.CreateBus())
            {
                //bus.SendReceive.Send("send.textmessage", new TextMessage { Text = "marsonshine" });
            }
        }
    }
}
