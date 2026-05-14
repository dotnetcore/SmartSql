using System;
using Xunit;

namespace SmartSql.Test.Integration.TypeHandlers
{
    public class CustomizeTypeHandlerTest : IntegrationTestBase
    {
        public CustomizeTypeHandlerTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void QueryByAnsiString()
        {
            var reqParams = new
            {
                AnsiString = "AnsiString"
            };
            var actual = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(CustomizeTypeHandlerTest),
                SqlId = nameof(QueryByAnsiString),
                Request = reqParams
            });
            Assert.Equal(reqParams.AnsiString, actual);
        }

        [Fact]
        public void QueryByAnsiStringFixedLength()
        {
            var reqParams = new
            {
                AnsiStringFixedLength = "AnsiStringFixedLength"
            };
            var actual = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(CustomizeTypeHandlerTest),
                SqlId = nameof(QueryByAnsiStringFixedLength),
                Request = reqParams
            });
            Assert.Equal(reqParams.AnsiStringFixedLength, actual);
        }
    }
}
