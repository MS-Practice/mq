using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MQ.Shared.RequestResponses
{
    public class CreateProductRequest
    {
        [JsonConstructor]
        public CreateProductRequest(string productName, DateTimeOffset produceDate, int qualityPeriod)
        {
            ProductName = productName;
            ProduceDate = produceDate;
            QualityPeriod = qualityPeriod;
        }

        public string ProductName { get; }
        public DateTimeOffset ProduceDate { get; }
        public int QualityPeriod { get; }
    }

    public class CreateProductResponse
    {
        [JsonConstructor]
        public CreateProductResponse(int productId, string code)
        {
            ProductId = productId;
            Code = code;
        }

        public int ProductId { get; }
        public string Code { get; }
    }
}
