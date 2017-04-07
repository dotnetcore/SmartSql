using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Dapper;
using System.Diagnostics;

namespace SmartSql.Tests
{
    public class SmartSqlMapper_Test : IDisposable
    {
        private static readonly SmartSqlMapper SqlMapper = new SmartSqlMapper();
        [Fact]
        public void Query()
        {
            int i = 0;
            for (i = 0; i < 10; i++)
            {

                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new
                    {
                        //Ids = new long[] { 1, 2, 3, 4 },
                        Name = "Name"
                    }
                });
            }
            Assert.True(i == 10);
        }
        [Fact]
        public async void QueryAsync()
        {
            int i = 0;
            var session = SqlMapper.CreateDbSession(DataSourceChoice.Read);
            session.BeginTransaction();
            for (i = 0; i < 10; i++)
            {
                var list = await SqlMapper.QueryAsync<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                }, session);
            }
            session.CommitTransaction();
            Assert.True(i == 10);
        }

        [Fact]
        public void Query_OnChangeConfig()
        {
            int i = 0;
            for (i = 0; i < 10; i++)
            {
                var list = SqlMapper.Query<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });
                Thread.Sleep(5000);
            }
        }


        [Fact]
        public void Insert()
        {
            int i = 0;
            try
            {
                SqlMapper.BeginTransaction();
                long id = 0;
                for (i = 0; i < 10; i++)
                {
                    id = SqlMapper.ExecuteScalar<long>(new RequestContext
                    {
                        Scope = "T_Test",
                        SqlId = "Insert",
                        Request = new T_Test { Name = $"Name-{id}" }
                    });
                }
                SqlMapper.CommitTransaction();
            }
            catch (Exception ex)
            {
                SqlMapper.RollbackTransaction();
            }
        }
        [Fact]
        public void StoredProcedure()
        {
            using (var dbSession = SqlMapper.CreateDbSession(DataSourceChoice.Read))
            {
                var guid = dbSession.Connection.QuerySingle<Guid>(sql: "GetId", commandType: System.Data.CommandType.StoredProcedure);
                Assert.NotNull(guid);
            }
        }

        public void Dispose()
        {
            SqlMapper.Dispose();
        }
    }
}
