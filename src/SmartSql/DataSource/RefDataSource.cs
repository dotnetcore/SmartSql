using System.ComponentModel;
using System.Data.Common;

namespace SmartSql.DataSource
{
    public class RefDataSource : AbstractDataSource
    {
        private readonly DbConnection _dbConnection;

        public RefDataSource(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            ConnectionString = _dbConnection.ConnectionString;
        }

        public override DbConnection CreateConnection()
        {
            return _dbConnection;
        }
    }
}