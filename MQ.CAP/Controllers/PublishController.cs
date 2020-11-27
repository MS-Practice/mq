using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQ.Domain;
using MQ.Domain.Repository;
using MQ.Shared;
using MQ.Shared.Messages;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MQ.CAP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublishController : ControllerBase
    {
        private readonly ILogger<TransferController> _logger;
        private readonly ICapPublisher _capPublisher;
        private readonly IUserRepository _userRepository;
        private const string connectionString = "server=192.168.3.125;database=db_platform_mq;uid=root;pwd=k8-sgM&W;charset='utf8';SslMode=None";
        public PublishController(ILogger<TransferController> logger,
            ICapPublisher capPublisher,
            IUserRepository userRepository)
        {
            _capPublisher = capPublisher;
            _logger = logger;
            _userRepository = userRepository;
        }


        [HttpGet("account/transfer/out/{money}")]
        public async Task<string> AccountsTransferOutTransaction(decimal money)
        {
            var user = new User
            {
                Id = 1,
                Name = "marsonshine",
                Account = new Account
                {
                    Id = 1,
                    BankNumber = "4430225366523211",
                    IdCard = "430602199310034532",
                    Price = 100000000.00M,
                    UpdationTime = DateTime.Now
                },
                IdCard = "430602199310034532"
            };
            using (var connection = new MySqlConnection(connectionString))
            {
                using var transaction = connection.BeginTransaction(_capPublisher, autoCommit: true);
                var userDb = await _userRepository.AddUserAsync(user, (IDbTransaction)transaction.DbTransaction);
                userDb.Account = user.Account;

                userDb.Account.Transferout(money);
                await _userRepository.UpdateAccountAsync(userDb.Account, (IDbTransaction)transaction.DbTransaction);

                await _capPublisher.PublishAsync(Consts.EventNames.AccountTransferIn, new TransferInAccountEvent(money, "summerzhu", user.Account.BankNumber));
            }
            return "转账成功";
        }

        [HttpGet("account/transfer/out/{money}/wrong")]
        public async Task<string> AccountsTransferOutTransactionThrowException(decimal money)
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
            using (var connection = new MySqlConnection(connectionString))
            {
                using var transaction = connection.BeginTransaction(_capPublisher, autoCommit: true);
                var userDb = await _userRepository.AddUserAsync(user, (IDbTransaction)transaction.DbTransaction);
                userDb.Account = user.Account;

                userDb.Account.Transferout(money);
                await _userRepository.UpdateAccountAsync(userDb.Account, (IDbTransaction)transaction.DbTransaction);
                throw new ArgumentException(nameof(money));
                await _capPublisher.PublishAsync(Consts.EventNames.AccountTransferIn, new TransferInAccountEvent(money, "summerzhu", "430602199710034532"));
            }
            return "转账成功";
        }
    }
}
