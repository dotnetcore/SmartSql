using System;
using SmartSql.Configuration.Tags;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class ForTest
    {
        protected ISqlMapper SqlMapper { get; }

        public ForTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void For_DirectValue_Test()
        {
            var forStr = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(ForTest),
                SqlId = nameof(For_DirectValue_Test),
                Request = new {Items = new[] {1, 2}, Separator = "-"}
            });
            Assert.Equal("1-2-", forStr);
        }

        [Fact]
        public void For_NotDirectValue_Test()
        {
            var forStr = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(ForTest),
                SqlId = nameof(For_NotDirectValue_Test),
                Request = new {Items = new[] {new {Id = 1}, new {Id = 2}}, Separator = "-"}
            });
            Assert.Equal("1-2-", forStr);
        }

        [Fact]
        public void For_NotDirectValue_WithKey_Test()
        {
            var forStr = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(ForTest),
                SqlId = nameof(For_NotDirectValue_WithKey_Test),
                Request = new {Items = new[] {new {Id = 1}, new {Id = 2}}, Separator = "-"}
            });
            Assert.Equal("1-2-", forStr);
        }

        [Fact]
        public void For_NotDirectNestValue_WithKey_Test()
        {
            var forStr = SqlMapper.QuerySingle<String>(new RequestContext
            {
                Scope = nameof(ForTest),
                SqlId = nameof(For_NotDirectNestValue_WithKey_Test),
                Request = new
                {
                    Items
                        = new[]
                        {
                            new {Info = new {Id = 1}},
                            new {Info = new {Id = 2}}
                        },
                    Separator = "-"
                }
            });
            Assert.Equal("1-2-", forStr);
        }
    }
}