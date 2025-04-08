using System.Data;

namespace MDM_KAI.Database
{
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// 데이터베이스 연결
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
    }
}
