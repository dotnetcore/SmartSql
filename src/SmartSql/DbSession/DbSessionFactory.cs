using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SmartSql.Configuration;
using SmartSql.DataSource;

namespace SmartSql.DbSession
{
    public class DbSessionFactory : IDbSessionFactory
    {
        public event DbSessionFactoryOpenedEventHandler Opened;
        public SmartSqlConfig SmartSqlConfig { get; }
        public DbSessionFactory(SmartSqlConfig smartSqlConfig)
        {
            SmartSqlConfig = smartSqlConfig;
        }

        public IDbSession Open()
        {
            var dbSession = new DefaultDbSession(SmartSqlConfig);
            Opened?.Invoke(this, new DbSessionFactoryOpenedEventArgs { DbSession = dbSession});
            return dbSession;
        }

        public IDbSession Open(AbstractDataSource dataSource)
        {
            var dbSession = Open();
            dbSession.SetDataSource(dataSource);
            return dbSession;
        }

        public IDbSession Open(string connectionString)
        {
            var dataSource = new WriteDataSource
            {
                Name = "Write",
                ConnectionString = connectionString,
                DbProvider = SmartSqlConfig.Database.DbProvider
            };
            return Open(dataSource);
        }
    }
}
