using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.EasyNetQ.Customer.Controllers
{
    [ApiController]
    [Route("subscrib")]
    public class SubscribController : ControllerBase
    {
        private readonly IBus _bus;
        public SubscribController(IBus bus)
        {
            _bus = bus;
        }


    }
}
