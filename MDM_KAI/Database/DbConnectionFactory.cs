using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace MDM_KAI.Database
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string? ConnectionString;

        public DbConnectionFactory(string _connectionstring)
        {
            this.ConnectionString = _connectionstring;
        }

        /// <summary>
        /// 데이터베이스 연결\
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            return new OracleConnection(ConnectionString);
        }
    }
}
