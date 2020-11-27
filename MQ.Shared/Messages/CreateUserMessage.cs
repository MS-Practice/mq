using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MQ.Shared.Messages
{
    public class CreateUserMessage
    {
        [JsonConstructor]
        public CreateUserMessage(string userName, int age, bool sex, string email, DateTime birthday)
        {
            UserName = userName;
            Age = age;
            Sex = sex;
            Email = email;
            Birthday = birthday;
        }
        public string UserName { get; }
        public int Age { get; }
        public bool Sex { get; }
        public string Email { get; }
        public DateTime Birthday { get; }

    }
}
