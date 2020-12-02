using EasyNetQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace MQ.EasyNetQ
{
    public interface ISubscriberBootstrapper
    {
        void Start();
    }

    public interface IAsyncSubscriberBootstrapper
    {
        Task StartAsync();
    }

    public abstract class SubscriberBootstrapperBase : ISubscriberBootstrapper, IAsyncSubscriberBootstrapper
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

        public void Start()
        {
            StartAsync().GetAwaiter().GetResult();
        }

        public abstract Task StartAsync();
    }
}
