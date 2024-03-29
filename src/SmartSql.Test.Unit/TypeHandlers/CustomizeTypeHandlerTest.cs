using System;
using System.Data;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    [Collection("GlobalSmartSql")]
    public class CustomizeTypeHandlerTest
    {
        protected ISqlMapper SqlMapper { get; }

        public CustomizeTypeHandlerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

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

