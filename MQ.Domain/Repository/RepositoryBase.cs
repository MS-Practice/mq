using MQ.Domain.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Domain.Repository
{
    // <Entity>
    public abstract class RepositoryBase : IRepository
    {
        public IDbConnection DbConnection { get; protected set; }

        public abstract Task<User> AddUserAsync(User user, IDbTransaction transaction = null);
        public abstract Task<bool> UpdateAccountAsync(Account account, IDbTransaction transaction = null);
    }
}
