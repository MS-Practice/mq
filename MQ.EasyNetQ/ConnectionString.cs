using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.EasyNetQ
{
    public class ConnectionString
    {
        private string connectionString;
        public ConnectionString() : this("") { }
        public ConnectionString(string con)
        {
            connectionString = con;
        }

        public static implicit operator string(ConnectionString connection) => connection.ToString();

        public static explicit operator ConnectionString(string connection) => new ConnectionString(connection);

        public ConnectionString LoadConfig(ConnectionStringConfig config)
        {
            throw new NotSupportedException();
        }

        public ConnectionString Append(string value)
        {
            connectionString += ";" + value;
            return this;
        }

        public override string ToString() => connectionString;
    }
}
