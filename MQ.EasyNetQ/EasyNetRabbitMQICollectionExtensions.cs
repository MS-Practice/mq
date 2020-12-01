using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQ.Shared.Messages;
using System;
using System.IO;
using System.Text;

namespace MQ.EasyNetQ
{
    public static class EasyNetRabbitMQICollectionExtensions
    {
        public static RabbitMQEasyNetBuilder EasyNetRabbitMQBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            string username = configuration["RabbitMQ:UserName"];
            string password = configuration["RabbitMQ:Password"];
            var connectionString = (ConnectionString)$"host={configuration["RabbitMQ:Server"]},{configuration["RabbitMQ:Server"]}:5673;username={username};password={password}";
            // publisherConfirms = true 为开启推送消息确认,建议开启,性能刚高
            // 因为不加上则当 rabbitmq 不可用时,发送消息会系统错误,而开启发送确认则不会,更具有伸缩性
            connectionString.Append("publisherConfirms=true");

            var bus = RabbitHutch.CreateBus(connectionString);
            // TODO: 待重构，最佳实践应该类似于autosubscribe以及respondor一样的编码方式
            // 利用 Naming conventions
            // https://github.com/EasyNetQ/EasyNetQ/wiki/Controlling-Queue-names
            // https://www.mariuszwojcik.com/tell-easynetq-bus-what-queue-name-to-use-for-your-messages/
            var exchange = bus.Advanced.ExchangeDeclare("go.exchange",ExchangeType.Direct,true);
            var queue = bus.Advanced.QueueDeclare("go.queue",true,false,false);
            bus.Advanced.Bind(exchange,queue,"go.queue");

            bus.Advanced.Consume(queue,(body,properties,info)=>{
                string json = UTF8Encoding.UTF8.GetString(body);
                var obj = System.Text.Json.JsonSerializer.Deserialize<CreateUserMessage>(json);
            });

            // var existingQueue = bus.Advanced.QueueDeclare("platform.queue.user", true, false, true);

            // bus.Advanced.Consume<CreateUserMessage>(existingQueue, (msg, info) =>
            // {
            //     var user = msg.Body;
            //     Console.WriteLine($"接收来自 Golang 发出的消息：{System.Text.Json.JsonSerializer.Serialize(user)} 时间:{DateTimeOffset.Now}");
            // });

            services.AddSingleton(bus);
            return new RabbitMQEasyNetBuilder(services);
        }

    }
}
