using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SmartSql.DbSession;
using System.Data.SqlClient;
using SmartSql.SqlMap;
using Dapper;
using SmartSql.Abstractions.Logging;

namespace SmartSql.Tests.DbSession
{
    public class DbConnectionSession_Test
    {
        static DbConnectionSession dbConnSession = new DbConnectionSession(NullLoggerFactory.Instance, SqlClientFactory.Instance, new DataSource
        {
            ConnectionString = "Data Source=.;database=TestDB;uid=sa;pwd=SmartSql.net",
            Name = "TestDB"
        });

        [Fact]
        public void NewInstance()
        {
            dbConnSession.OpenConnection();
            var list = dbConnSession.Connection.Query<T_Test>("Select * From T_Test");
            dbConnSession.CloseConnection();
            Assert.Null(dbConnSession.Connection);
        }
        [Fact]
        public void Insert()
        {
            dbConnSession.BeginTransaction();
            for (int i = 0; i < 100; i++)
            {
                long Id = dbConnSession.Connection.ExecuteScalar<long>("Insert T_Test(Name) Values (@Name); Select @@IDENTITY", new T_Test
                {
                    Name = $"TestName-{i}"
                }, dbConnSession.Transaction);
            }
            dbConnSession.CommitTransaction();
        }
    }

    public class T_Test
    {
        public long Id { get; set; }
        public String Name { get; set; }
    }
}


