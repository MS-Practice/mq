using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQ.Shared.RequestResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer.RequestHandlers
{
    public class CreateUserRequestHandler : ResponderBase,IResponder
    {
        public CreateUserRequestHandler(IBus bus) : base(bus)
        {
        }

        public override void Subscribe()
        {
            Bus.Rpc.Respond<CreateUserRequest, CreateUserReponse>(Response);
        }

        private CreateUserReponse Response(CreateUserRequest createUser)
        {
            var response = new CreateUserReponse(1, "430602111366523336");
            Logger.LogInformation($"接收消息：{System.Text.Json.JsonSerializer.Serialize(createUser)} 时间:{DateTimeOffset.Now}{Environment.NewLine}响应消息:{System.Text.Json.JsonSerializer.Serialize(response)}");
            return response;
        }
    }
}
