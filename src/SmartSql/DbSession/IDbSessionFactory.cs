using SmartSql.Configuration;
using SmartSql.DataSource;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartSql.DbSession
{
    public interface IDbSessionFactory
    {
        SmartSqlConfig SmartSqlConfig { get; }
        IDbSession Open();
        IDbSession Open(String connectionString);
        IDbSession Open(AbstractDataSource dataSource);
    }
}
