using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class OrderByTests
    {
        private RequestContext CreateContext(string property, object value)
        {
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd(property, value);
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();
            return context;
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyIsSingleOrderBy()
        {
            var tag = new OrderBy { Property = "OrderBy" };
            var context = CreateContext("OrderBy", new KeyValuePair<string, string>("Name", "ASC"));

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyIsOrderByCollection()
        {
            var tag = new OrderBy { Property = "OrderBy" };
            var orderBys = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "ASC"),
                new KeyValuePair<string, string>("Age", "DESC")
            };
            var context = CreateContext("OrderBy", orderBys);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new OrderBy { Property = "OrderBy" };
            var context = CreateContext("OrderBy", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsEmptyOrderByCollection()
        {
            var tag = new OrderBy { Property = "OrderBy" };
            var context = CreateContext("OrderBy", new List<KeyValuePair<string, string>>());

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_BuildOrderBySql_When_SingleOrderBy()
        {
            var tag = new OrderBy { Property = "OrderBy" };
            var context = CreateContext("OrderBy", new KeyValuePair<string, string>("Name", "ASC"));

            tag.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("Name ASC");
        }

        [Fact]
        public void Should_BuildOrderBySql_When_MultipleOrderBys()
        {
            var tag = new OrderBy { Property = "OrderBy" };
            var orderBys = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Name", "ASC"),
                new KeyValuePair<string, string>("Age", "DESC")
            };
            var context = CreateContext("OrderBy", orderBys);

            tag.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("Name ASC");
            sql.Should().Contain("Age DESC");
            sql.Should().Contain(",");
        }
    }
}
