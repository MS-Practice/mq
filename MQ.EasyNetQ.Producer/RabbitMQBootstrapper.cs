using EasyNetQ;
using MQ.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Producer
{
    public class RabbitMQBootstrapper
    {
        public static void Start()
        {
            using (var bus = CreateBus())
            {
                var input = "";
                Console.WriteLine("Enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    //bus.PubSub.Publish(new TextMessage
                    //{
                    //    Text = input
                    //});
                }
            }
        }

        public static IBus CreateBus() => RabbitHutch.CreateBus("host=192.168.3.67;timeout=10");
    }
}
