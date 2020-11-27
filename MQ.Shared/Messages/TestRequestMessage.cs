using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Shared.Messages
{
    public class TestRequestMessage
    {
        public string Text { get; set; }
    }

    public class TestResponseMessage
    {
        public string Text { get; set; }
    }
}
