using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddSingleton(RabbitHutch.CreateBus(connectionString));
            return new RabbitMQEasyNetBuilder(services);
        }

    }
}
