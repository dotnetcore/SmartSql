using System;
using System.Data.Common;

namespace SmartSql.DataSource
{
    /// <summary>
    /// 数据源
    /// </summary>
    public abstract class AbstractDataSource
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// 数据源链接字符串
        /// </summary>
        public String ConnectionString { get; set; }

        public DbProvider DbProvider { get; set; }

        public virtual DbConnection CreateConnection()
        {
            var dbConnection= DbProvider.Factory.CreateConnection();
            dbConnection.ConnectionString = ConnectionString;
            return dbConnection;
        }
    }
}
