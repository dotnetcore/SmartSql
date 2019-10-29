using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class PlaceholderTest
    {
        protected ISqlMapper SqlMapper { get; }

        public PlaceholderTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void Placeholder_Test()
        {
            var UserList = SqlMapper.Query<User>(new RequestContext
            {
                Scope = nameof(PlaceholderTest),
                SqlId = "Query",
                Request = new {Placeholder = "Select TUE.UserId From T_UserExtendedInfo as TUE"}
            });
            Assert.True(true);
        }

        [Fact]
        public void Nest_Test()
        {
            var reqParams = new {Nest = new {Sql = "Nest.Sql"}};
            var result = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(PlaceholderTest),
                SqlId = nameof(Nest_Test),
                Request = reqParams
            });
            Assert.Equal(reqParams.Nest.Sql, result);
        }
    }
}