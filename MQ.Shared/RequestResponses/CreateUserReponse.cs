using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MQ.Shared.RequestResponses
{
    public class CreateUserReponse
    {
        [JsonConstructor]
        public CreateUserReponse(int userId, string idCard)
        {
            UserId = userId;
            IdCard = idCard;
        }

        public int UserId { get;}
        public string IdCard { get;}
    }
}
