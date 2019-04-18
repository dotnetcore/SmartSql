using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    public class SqlMapperTest : AbstractXmlConfigBuilderTest
    {
        protected ISqlMapper SqlMapper { get; }
        public SqlMapperTest()
        {
            SqlMapper = BuildSqlMapper(this.GetType().FullName);
        }
        [Fact]
        public async Task QueryAsync()
        {
            var list = await SqlMapper.QueryAsync<dynamic>(new RequestContext
            {
                RealSql = "SELECT Top (5) T.* From T_AllPrimitive T With(NoLock)"
            });

            Assert.NotNull(list);
        }
    }
}
