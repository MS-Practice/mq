using EasyNetQ;
using MQ.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer.Features
{
    public class RequestResponsePattern
    {
        public static void Start()
        {
            //var bus = RabbitMQBootstrapper.CreateBus();
            //{
            //    var request = new TestRequestMessage { Text = "marsonshine request" };

            //    var r = bus.Request<TestRequestMessage, TestResponseMessage>(request);
            //    //r.ContinueWith(response =>
            //    //{
            //    //    Console.WriteLine($"获取响应体：{response.Result.Text}");
            //    //});
            //    Console.ReadLine();
            //    bus.Dispose();
            //}
        }
    }
}
