using System;
using System.Text.Json.Serialization;

namespace MQ.Shared.RequestResponses
{
    public class CreateUserRequest
    {
        [JsonConstructor]
        public CreateUserRequest(string userName, int age, bool sex, string email, DateTime birthday)
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
