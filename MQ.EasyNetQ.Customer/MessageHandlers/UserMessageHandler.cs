using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Consumer;
using MQ.Shared.Messages;
using MQ.Shared.RabbitMQ;
using MQ.Shared.RequestResponses;

namespace MQ.EasyNetQ.Customer.MessageHandlers
{
    public class UserMessageHandler : IConsumeAsync<CreateUserMessage>
    {
        private readonly ILoggerFactory _loggerFactory;
        public UserMessageHandler(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public ILogger Logger => _loggerFactory.CreateLogger<UserMessageHandler>();
        [ForTopic(Consts.Topic.User)]
        public async Task ConsumeAsync(CreateUserMessage message, CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"接收消息：{JsonSerializer.Serialize(message)} 时间:{DateTimeOffset.Now}");
            await Task.Yield();
        }
    }
}
