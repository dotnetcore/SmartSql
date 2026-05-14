using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class PlaceholderTests
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

        private RequestContext CreateEmptyContext()
        {
            var sqlParams = new SqlParameterCollection();
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();
            return context;
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyExists()
        {
            var tag = new Placeholder { Property = "Name" };
            var context = CreateContext("Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyDoesNotExist()
        {
            var tag = new Placeholder { Property = "Name" };
            var context = CreateEmptyContext();

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyExistsWithNullValue()
        {
            var tag = new Placeholder { Property = "Name" };
            var context = CreateContext("Name", null);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_AppendValueToSqlBuilder_When_ConditionIsMet()
        {
            var tag = new Placeholder { Property = "Name", Prepend = "AND Name=" };
            var context = CreateContext("Name", "SmartSql");

            tag.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("SmartSql");
        }

        [Fact]
        public void Should_NotAppendToSqlBuilder_When_ConditionIsNotMet()
        {
            var tag = new Placeholder { Property = "Name", Prepend = "AND Name=" };
            var context = CreateEmptyContext();

            tag.BuildSql(context);

            context.SqlBuilder.ToString().Should().BeEmpty();
        }

        [Fact]
        public void Should_AppendPrependToSqlBuilder_When_ConditionIsMet()
        {
            var tag = new Placeholder { Property = "Name", Prepend = "AND Name=" };
            var context = CreateContext("Name", "SmartSql");

            tag.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("AND Name=");
        }
    }
}
