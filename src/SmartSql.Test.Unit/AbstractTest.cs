using SmartSql.DataSource;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit
{
    public abstract class AbstractTest : IDisposable
    {
        protected String DbType => "SqlServer";
        protected String ConnectionString => "Data Source=.;Initial Catalog=SmartSqlTestDB;Integrated Security=True";
        protected IDbSessionFactory DbSessionFactory { get; }
        public AbstractTest()
        {
            DbSessionFactory = SmartSqlBuilder.AddDataSource(DbProvider.SQLSERVER, ConnectionString)
                .UseCache().Build().GetDbSessionFactory();
        }

        public void Dispose()
        {
            SmartSqlContainer.Instance.Dispose();
        }
    }
}
