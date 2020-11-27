using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.EasyNetQ
{
    public interface IResponder
    {
        void Subscribe();
    }

    public abstract class ResponderBase : IResponder
    {
        private readonly IBus _bus;
        private ILogger _logger;

        public IBus Bus => _bus;

        public ILogger Logger
        {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        protected ResponderBase(IBus bus)
        {
            _bus = bus;
        }

        public abstract void Subscribe();
    }
}
