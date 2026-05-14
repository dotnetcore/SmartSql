using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class WhereTests
    {
        private RequestContext CreateContext(string property = null, object value = null)
        {
            var sqlParams = new SqlParameterCollection();
            if (property != null)
            {
                sqlParams.TryAdd(property, value);
            }

            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();
            return context;
        }

        [Fact]
        public void Should_ReturnTrue_When_ChildTagMatchesCondition()
        {
            var where = new Where { ChildTags = new List<ITag>() };
            where.ChildTags.Add(new SqlText("Name = @Name", "@"));

            var context = CreateContext();

            var result = where.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ConditionalChildMatches()
        {
            var where = new Where { ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            where.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "SmartSql");

            var result = where.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NoChildTagMatches()
        {
            var where = new Where { ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            where.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "");

            var result = where.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_PrependWhere_When_BuildingSql()
        {
            var where = new Where { ChildTags = new List<ITag>() };
            where.ChildTags.Add(new SqlText("Name = @Name", "@"));

            var context = CreateContext();

            where.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("Where");
            sql.Should().Contain("Name = @Name");
        }

        [Fact]
        public void Should_NotBuildSql_When_NoChildMatches()
        {
            var where = new Where { ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            where.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "");

            where.BuildSql(context);

            context.SqlBuilder.ToString().Should().BeEmpty();
        }

        [Fact]
        public void Should_Throw_When_MatchedLessThanMin()
        {
            var statement = new Statement
            {
                Id = "TestStatement",
                SqlMap = new SqlMap { Scope = "TestScope" }
            };
            var where = new Where { Min = 2, ChildTags = new List<ITag>(), Statement = statement };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            where.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "value");

            var act = () => where.IsCondition(context);

            act.Should().Throw<TagMinMatchedFailException>();
        }

        [Fact]
        public void Should_HavePrependSetToWhere()
        {
            var where = new Where();

            where.Prepend.Should().Be("Where");
        }
    }
}
