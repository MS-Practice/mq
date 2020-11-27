using EasyNetQ;
using Microsoft.Extensions.Logging;
using MQ.Shared.RequestResponses;
using System;

namespace MQ.EasyNetQ.Customer.RequestHandlers
{
    public class CreateProductRequestHandler : ResponderBase,IResponder
    {

        public CreateProductRequestHandler(IBus bus)
            :base(bus)
        {

        }

        public override void Subscribe()
        {
            Bus.Rpc.Respond<CreateProductRequest, CreateProductResponse>(Response);
        }

        private CreateProductResponse Response(CreateProductRequest request)
        {
            var response = new CreateProductResponse(1, "basketball");
            Logger.LogInformation($"接收消息：{System.Text.Json.JsonSerializer.Serialize(request)} 时间:{DateTimeOffset.Now}{Environment.NewLine}响应消息:{System.Text.Json.JsonSerializer.Serialize(response)}");
            return response;
        }
    }
}
