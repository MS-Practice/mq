using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MQ.Domain.Data
{
    public static class DatabaseExtenssions
    {
        public static int ExecuteNonQuery(this IDbConnection connection, string sql, IDbTransaction transaction = null,
            params object[] sqlParams)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            foreach (var param in sqlParams)
            {
                command.Parameters.Add(param);
            }

            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            return command.ExecuteNonQuery();
        }

        public static long ExecuteQueryReturnIdentity(this IDbConnection connection, string sql, IDbTransaction transaction = null,
            params object[] sqlParams)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            foreach (var param in sqlParams)
            {
                command.Parameters.Add(param);
            }

            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.ExecuteNonQuery();
            command.CommandText = "SELECT LAST_INSERT_ID();";
            return Convert.ToInt64((ulong)command.ExecuteScalar());
        }
    }
}
