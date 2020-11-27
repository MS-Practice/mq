using EasyNetQ;
using MQ.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Producer.Features
{
    public class SendReceivePattern
    {
        public static void Start()
        {
            using (var bus = RabbitMQBootstrapper.CreateBus())
            {
                //bus.SendReceive.Receive<TextMessage>("send.textmessage", message =>
                //{
                //    Console.WriteLine($"接收消息：{message.Text}");
                //});
            }
        }
    }
}
