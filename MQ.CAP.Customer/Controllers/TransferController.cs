using System;
using System.Collections.Generic;
using System.Linq;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQ.Domain;
using MQ.Shared;
using MQ.Shared.Messages;

namespace MQ.CAP.Customer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController : ControllerBase
    {

        private readonly ILogger<TransferController> _logger;

        public TransferController(ILogger<TransferController> logger)
        {
            _logger = logger;
        }

        [NonAction]
        [CapSubscribe(Consts.EventNames.AccountTransferIn)]
        public void Subscriber(TransferInAccountEvent eventEntity)
        {
            _logger.LogInformation($"消息体 == 姓名:{eventEntity.UserName} 银行账号:{eventEntity.AccountNumber} 转账类型:转入 金额:{eventEntity.Price}");
            _logger.LogInformation($"根据姓名和银行账号查询数据库信息...");

            var anotherUser = new User
            {
                Id = 2,
                Name = eventEntity.UserName,
                Account = new Account
                {
                    Id = 2,
                    BankNumber = eventEntity.AccountNumber,
                    Price = 10045200.00M,
                    UpdationTime = DateTime.Now
                }
            };

            anotherUser.Account.TransferIn(eventEntity.Price);
            _logger.LogInformation("$转入成功!");
        }
    }
}
