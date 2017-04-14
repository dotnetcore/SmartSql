using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace SmartSql.Tests
{
    public class SmartSqlMapperAsync_Test : TestBase
    {

        [Fact]
        public async void QueryAsync()
        {
            int i = 0;
            SqlMapper.BeginTransaction();
            for (i = 0; i < 10; i++)
            {

                var list = await SqlMapper.QueryAsync<T_Test>(new RequestContext
                {
                    Scope = "T_Test",
                    SqlId = "GetList",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });

            }
            SqlMapper.CommitTransaction();
            Assert.True(i == 10);
        }
    }
}
