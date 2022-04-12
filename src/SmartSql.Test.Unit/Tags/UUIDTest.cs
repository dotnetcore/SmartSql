using System;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class UUIDTest
    {
        protected ISqlMapper SqlMapper { get; }

        public UUIDTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        // TODO
        [Fact(Skip = "TODO")]
        public void GetUUID()
        {
            var msg = SqlMapper.ExecuteScalar<Guid>(new RequestContext
            {
                Scope = nameof(UUIDTest),
                SqlId = "GetUUID"
            });
            Assert.True(true);
        }
        // TODO
        [Fact(Skip = "TODO")]
        public void GetUUIDToN()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(UUIDTest),
                SqlId = "GetUUIDToN"
            });
            Assert.True(true);
        }
        
    }
}