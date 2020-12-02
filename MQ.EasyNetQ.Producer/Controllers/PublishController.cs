using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQ.Shared.Messages;
using MQ.Shared.RabbitMQ;
using MQ.Shared.RequestResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Producer.Controllers
{
    [ApiController]
    [Route("publish")]
    public class PublishController : ControllerBase
    {
        private readonly IBus _bus;
        private ILogger _logger;
        public ILogger Logger
        {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        public PublishController(IBus bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
            _logger = loggerFactory.CreateLogger<PublishController>();
        }
        [HttpPost("user/create")]
        public async Task<bool> CreateUser([FromBody] CreateUserMessage createUser)
        {
            await _bus.PubSub.PublishAsync(createUser, topic: Consts.Topic.User);
            return true;
        }

        [HttpPost("user/create/queue")]
        public async Task<bool> CreateUserQueue([FromBody] CreateUserMessage createUser)
        {
            var queue = await _bus.Advanced.QueueDeclareAsync("platform.queue.createuser", true, false, true);
            //_bus.Advanced.PublishAsync()
            //await _bus.PubSub.PublishAsync(createUser, topic: Consts.Topic.User);
            return true;
        }

        [HttpPost("user/request/create")]
        public async Task<CreateUserReponse> CreateUser([FromBody] CreateUserRequest createUser)
        {
            return await _bus.Rpc.RequestAsync<CreateUserRequest, CreateUserReponse>(createUser);
        }

        [HttpPost("user/create/ack")]
        public async Task<bool> CreateUserAck([FromBody] CreateUserMessage message)
        {
            await _bus.PubSub.PublishAsync(message, topic: Consts.Topic.User)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted && !task.IsCompletedSuccessfully)
                    {
                        Logger.LogInformation($"消息 {message.UserName} 推送 broken 失败");
                    }
                    if (task.IsCompletedSuccessfully)
                    {
                        Logger.LogInformation($"消息 {message.UserName} 推送确认");
                    }
                    if (task.IsFaulted)
                    {
                        Logger.LogError($"系统错误:{task.Exception.Message}");
                    }
                });

            return true;
        }
        [HttpPost("product/request/create")]
        public async Task<CreateProductResponse> CreateUser([FromBody] CreateProductRequest createProduct)
        {
            return await _bus.Rpc.RequestAsync<CreateProductRequest, CreateProductResponse>(createProduct);
        }
    }
}
