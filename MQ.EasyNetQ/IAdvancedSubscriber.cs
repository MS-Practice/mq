using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace MQ.EasyNetQ
{
    public interface IAdvancedSubscriber
    {
        void Subscribe();
    }

    public interface IAsyncAdvancedSubscriber
    {
        Task SubscribeAsync();
    }

    public abstract class SubscriberBootstrapperBase : IAdvancedSubscriber, IAsyncAdvancedSubscriber
    {
        private readonly IBus _bus;
        private ILogger _logger;

        public IBus Bus => _bus;

        public ILogger Logger
        {
            get { return _logger ??= NullLogger.Instance; }
            set { _logger = value; }
        }

        protected SubscriberBootstrapperBase(IBus bus)
        {
            _bus = bus;
        }

        public void Subscribe()
        {
            SubscribeAsync().GetAwaiter().GetResult();
        }

        public abstract Task SubscribeAsync();
    }
}
