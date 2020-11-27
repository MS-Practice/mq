using EasyNetQ.AutoSubscribe;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MQ.EasyNetQ
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseAutoSubscriber(this IApplicationBuilder app,Assembly[] assemblies)
        {
            var subscriber = app.ApplicationServices.GetService<AutoSubscriber>();
            subscriber.Subscribe(assemblies);

            var requests = app.ApplicationServices.GetServices<IResponder>();
            foreach (var request in requests)
            {
                request.Subscribe();
            }
        }
    }
}
