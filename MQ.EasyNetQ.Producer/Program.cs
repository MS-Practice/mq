using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQ.EasyNetQ.Producer.Features;
using MQ.Shared.Messages;

namespace MQ.EasyNetQ.Producer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //using (var bus = RabbitHutch.CreateBus("host=192.168.3.67"))
            //{
            //    bus.Rpc.Respond<string, string>(request => {
            //        Console.Out.WriteLine("get request:" + request);
            //        return request + " reply: summer zhu";
            //    });

            //    bus.Rpc.Respond<TestRequestMessage, TestResponseMessage>(request =>
            //    {
            //        Console.Out.WriteLine("get TestRequestMessage:" + System.Text.Json.JsonSerializer.Serialize(request));
            //        return new TestResponseMessage { Text = request.Text + " reply: summer zhu" };
            //    });
            //    Console.ReadLine();
            //}
            ////RabbitMQBootstrapper.Start();
            //RequestResponsePattern.Start();
            //SendReceivePattern.Start();
            CreateHostBuilder(args).Build().Run();
            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://*:5000");
                });
    }
}
