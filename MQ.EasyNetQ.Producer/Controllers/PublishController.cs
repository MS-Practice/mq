using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using MQ.Shared.Messages;
using MQ.Shared.RabbitMQ;
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
        public PublishController(IBus bus)
        {
            _bus = bus;
        }
        [HttpPost("user/create")]
        public async Task<bool> CreateUser([FromBody] CreateUserMessage createUser)
        {
            await _bus.PubSub.PublishAsync(createUser,topic:Consts.Topic.User);
            return true;
        }
    }
}
