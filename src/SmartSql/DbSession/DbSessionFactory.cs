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
        public SmartSqlConfig SmartSqlConfig { get; }
        public DbSessionFactory(SmartSqlConfig smartSqlConfig)
        {
            SmartSqlConfig = smartSqlConfig;
        }

        public IDbSession Open()
        {
            var dbSession = new DefaultDbSession(SmartSqlConfig);
            SmartSqlConfig.CacheManager.BindSessionEventHandler(dbSession);
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
