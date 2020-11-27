using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;

namespace MQ.EasyNetQ
{
    public class RabbitMQEasyNetBuilder
    {
        private readonly IServiceCollection _services;
        public RabbitMQEasyNetBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public void UseAutoSubscriber(string subscriptionIdPrefix)
        {
            _services.AddSingleton<MessageDispatcher>();
            _services.AddSingleton<AutoSubscriber>(provider =>
            {
                var subscriber = new AutoSubscriber(provider.GetRequiredService<IBus>(), subscriptionIdPrefix)
                {
                    AutoSubscriberMessageDispatcher = provider.GetRequiredService<MessageDispatcher>()
                };
                return subscriber;
            });
        }
    }
}
