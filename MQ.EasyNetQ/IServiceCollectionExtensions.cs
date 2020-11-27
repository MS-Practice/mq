using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MQ.EasyNetQ
{
    public static class IServiceCollectionExtensions
    {
        public static RabbitMQEasyNetBuilder EasyNetRabbitMQBuilder(this IServiceCollection services, IConfiguration configuration)
        {
            string username = configuration["RabbitMQ:UserName"];
            string password = configuration["RabbitMQ:Password"];
            string connectionString = $"host={configuration["RabbitMQ:Server"]};username={username};password={password}";
            services.AddSingleton(RabbitHutch.CreateBus(connectionString));
            return new RabbitMQEasyNetBuilder(services);
        }

    }
}
