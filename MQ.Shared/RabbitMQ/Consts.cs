using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Shared.RabbitMQ
{
    public class Consts
    {
        public class Topic
        {
            internal const string Prefix = "MQ.EasyNetQ.Topic";
            public const string User = Prefix + ".User";
        }

        public class Queue
        {
            internal const string Prefix = "platform.queue";
            public const string User = Prefix + ".user";
        }
    }
}
