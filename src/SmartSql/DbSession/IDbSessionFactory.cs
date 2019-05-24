using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.DbSession
{
    public class DbSessionFactoryOpenedEventArgs : EventArgs
    {
        public IDbSession DbSession { get; set; }
    }
    public delegate void DbSessionFactoryOpenedEventHandler(object sender, DbSessionFactoryOpenedEventArgs eventArgs);
    
    public interface IDbSessionFactory
    {
        event DbSessionFactoryOpenedEventHandler Opened;
        SmartSqlConfig SmartSqlConfig { get; }
        IDbSession Open();
        IDbSession Open(String connectionString);
        IDbSession Open(AbstractDataSource dataSource);
    }
}
