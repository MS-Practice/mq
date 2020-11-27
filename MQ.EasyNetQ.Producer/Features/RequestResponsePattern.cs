using EasyNetQ;
using MQ.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Producer.Features
{
    public class RequestResponsePattern
    {
        public static void Start()
        {
            //using (var bus = RabbitMQBootstrapper.CreateBus())
            //{
            //    var response = bus.Respond<TestRequestMessage, TestResponseMessage>(SenRequest);
            //}              
        }

        private static TestResponseMessage SenRequest(TestRequestMessage request)
        {
            Console.WriteLine($"接收到的请求消息：{request.Text}");
            return new TestResponseMessage { Text = $"Reply - {request.Text}" };
        }
    }
}
