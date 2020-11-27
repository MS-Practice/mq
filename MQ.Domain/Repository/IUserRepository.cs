using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Domain.Repository
{
    public interface IUserRepository : IRepository
    {
        Task<User> AddUserAsync(User user, IDbTransaction transaction = null);
        Task<bool> UpdateAccountAsync(Account account, IDbTransaction dbTransaction = null);
    }
}
