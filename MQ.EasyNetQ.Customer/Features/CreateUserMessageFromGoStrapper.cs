using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using MQ.Shared.Messages;
using MQ.Shared.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer.Features
{
    public class CreateUserMessageFromGoStrapper : SubscriberBootstrapperBase, IAdvancedSubscriber, IAsyncAdvancedSubscriber
    {
        public CreateUserMessageFromGoStrapper(IBus bus, ILoggerFactory loggerFactory) : base(bus)
        {
            Logger = loggerFactory.CreateLogger<CreateUserMessageFromGoStrapper>();
        }

        public override async Task SubscribeAsync()
        {
            var exchange = await ExchangeDeclareAsync();
            var queue = await QueueDeclareAsync();
            await BindingAsync(exchange, queue);
            Consumer(queue);
        }

        private void Consumer(IQueue queue)
        {
            Bus.Advanced.Consume(queue, (body, properties, info) =>
            {
                string json = Encoding.UTF8.GetString(body);
                var userMessage = System.Text.Json.JsonSerializer.Deserialize<CreateUserMessage>(body);
                Logger.LogInformation($"接收消息：{System.Text.Json.JsonSerializer.Serialize(userMessage)} 时间:{DateTimeOffset.Now}");
            });
        }

        private async Task BindingAsync(IExchange exchange, IQueue queue)
        {
            _ = await Bus.Advanced.BindAsync(exchange, queue, Consts.RouterKeys.CreateUser, headers: null);
        }

        private async Task<IQueue> QueueDeclareAsync()
        {
            var queue = await Bus.Advanced.QueueDeclareAsync(Consts.Queue.User, durable: true, exclusive: false, autoDelete: false);
            return queue;
        }

        private async Task<IExchange> ExchangeDeclareAsync()
        {
            var exchange = await Bus.Advanced.ExchangeDeclareAsync(Consts.Exchange.User, ExchangeType.Direct, durable: true, autoDelete: false);
            return exchange;
        }
    }
}
