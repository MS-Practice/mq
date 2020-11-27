using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("user/request/create")]
        public async Task<CreateUserReponse> CreateUser([FromBody] CreateUserRequest createUser)
        {
            return await _bus.Rpc.RequestAsync<CreateUserRequest, CreateUserReponse>(createUser);
        }

        [HttpPost("product/request/create")]
        public async Task<CreateProductResponse> CreateUser([FromBody] CreateProductRequest createProduct)
        {
            return await _bus.Rpc.RequestAsync<CreateProductRequest, CreateProductResponse>(createProduct);
        }
    }
}
