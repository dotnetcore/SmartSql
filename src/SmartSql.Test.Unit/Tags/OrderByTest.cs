using System;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    [Collection("GlobalSmartSql")]
    public class OrderByTest
    {
        protected ISqlMapper SqlMapper { get; }

        public OrderByTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void OrderBy()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(OrderByTest),
                SqlId = "OrderBy",
                Request = new
                {
                    OrderBy = new KeyValuePair<String, String>("Id", "Desc")
                }
            });
            Assert.Equal(" Order By Id Desc", msg);
        }

        [Fact]
        public void OrderByMulti()
        {
            var msg = SqlMapper.ExecuteScalar<String>(new RequestContext
            {
                Scope = nameof(OrderByTest),
                SqlId = "OrderBy",
                Request = new
                {
                    OrderBy = new Dictionary<string, string>
                    {
                        {"Id", "Desc"},
                        {"Name", "Asc"},
                    }
                }
            });
            Assert.Equal(" Order By Id Desc,Name Asc", msg);
        }
    }
}