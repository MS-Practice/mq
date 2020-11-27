using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQ.EasyNetQ.Customer.Features;
using MQ.Shared.Messages;

namespace MQ.EasyNetQ.Customer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //using (var bus = RabbitMQBootstrapper.CreateBus())
            //{
            //    bus.PubSub.Subscribe<TextMessage>("test", HandleTextMessage);

            //    Console.WriteLine("Listening for messages. Hit <return> to quit.");
            //    Console.ReadLine();
            //}
            //using (var bus = RabbitHutch.CreateBus("host=192.168.3.67"))
            //{
            //    var response = bus.Rpc.Request<string, string>("marsonshine");
            //    Console.Out.WriteLine("response:" + response);

            //    var responseMessage = bus.Rpc.Request<TestRequestMessage, TestResponseMessage>(new TestRequestMessage { Text = "marsonshine" });
            //    Console.Out.WriteLine("response:" + System.Text.Json.JsonSerializer.Serialize(responseMessage));
            //}
            //RequestResponsePattern.Start();
            //SendReceivePattern.Start();
            CreateHostBuilder(args).Build().Run();
            Console.ReadLine();
        }

        static void HandleTextMessage(TextMessage textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got message: {0}", textMessage.Text);
            Console.ResetColor();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logger => {
                    logger.SetMinimumLevel(LogLevel.Information);
                    logger.ClearProviders();
                    logger.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls("http://*:5001");
                });
    }
}
