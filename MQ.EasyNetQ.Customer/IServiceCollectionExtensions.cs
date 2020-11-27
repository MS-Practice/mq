using Microsoft.Extensions.DependencyInjection;
using MQ.EasyNetQ.Customer.MessageHandlers;
using MQ.EasyNetQ.Customer.RequestHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer
{
    public static class IServiceCollectionExtensions
    {
        public static void AddSubscriptionServices(this IServiceCollection services)
        {
            services.AddTransient<UserMessageHandler>();
            services.AddTransient<IResponder, CreateUserRequestHandler>();
            services.AddTransient<IResponder, CreateProductRequestHandler>();
        }
    }
}
