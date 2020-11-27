using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQ.Domain;
using MQ.Shared;
using MQ.Shared.Messages;

namespace MQ.CAP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ILogger<TransferController> _logger;
        private readonly ICapPublisher _capPublisher;

        public TransferController(ILogger<TransferController> logger,
            ICapPublisher capPublisher)
        {
            _logger = logger;
            _capPublisher = capPublisher;
        }
        [HttpGet("account/transfer/out/{money}")]
        public async Task<string> AccountsTransferOutWithoutTransaction(decimal money)
        {
            var user = new User
            {
                Id = 1,
                Name = "marsonshine",
                Account = new Account
                {
                    Id = 1,
                    BankNumber = "430602199310034532",
                    Price = 100000000.00M,
                    UpdationTime = DateTime.Now
                }
            };

            user.Account.Transferout(money);

            await _capPublisher.PublishAsync(Consts.EventNames.AccountTransferIn, new TransferInAccountEvent(money, "summerzhu", "430602199710034532"));

            return "转账成功";
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
        [NonAction]
        [CapSubscribe(name: Consts.EventNames.AccountTransferIn, Group = "account.transfer")]
        public void SubscriberWithOnlyOne(TransferInAccountEvent eventEntity)
        {
            _logger.LogInformation("不同 group 中的 queue message 只能被消费一次");
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
