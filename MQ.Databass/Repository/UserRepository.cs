using MQ.Domain;
using MQ.Domain.Data;
using MQ.Domain.Repository;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Databass.Repository
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository()
        {
            DbConnection = new MySqlConnection("server=192.168.3.125;database=db_platform_mq;uid=root;pwd=k8-sgM&W;charset='utf8';SslMode=None");
        }
        public override async Task<User> AddUserAsync(User user, IDbTransaction transaction = null)
        {
            object[] sqlParams = {
                new MySqlParameter("@name", user.Name),
                new MySqlParameter("@idcard", user.IdCard),
            };
            long id;
            if (transaction == null)
            {
                id = DbConnection.ExecuteQueryReturnIdentity($"INSERT INTO `tb_user` (Name, IdCard) VALUES (@name, @idcard);", sqlParams: sqlParams);
            }
            else
            {
                var conn = transaction?.Connection;
                id = conn.ExecuteQueryReturnIdentity($"INSERT INTO `tb_user` (Name, IdCard) VALUES (@name, @idcard);", transaction, sqlParams: sqlParams);
            }

            user.Id = (int)id;
            return await Task.FromResult(user);
        }

        public override async Task<bool> UpdateAccountAsync(Account account, IDbTransaction transaction = null)
        {
            object[] sqlParams = {
                new MySqlParameter("@bankNumber", account.BankNumber),
                new MySqlParameter("@idcard", account.IdCard),
                new MySqlParameter("@price", account.Price)
            };
            int r;
            if (transaction == null)
            {
                r = DbConnection.ExecuteNonQuery($"UPDATE `tb_account` SET price=@price,update_time=now() WHERE idcard=@idcard AND bank_number=@bankNumber;", sqlParams: sqlParams);
            }
            else
            {
                var conn = transaction?.Connection;
                r = conn.ExecuteNonQuery($"UPDATE `tb_account` SET price=@price,update_time=now() WHERE idcard=@idcard AND bank_number=@bankNumber;", transaction, sqlParams: sqlParams);
            }

            return await Task.FromResult(r > 0);
        }
    }
}
