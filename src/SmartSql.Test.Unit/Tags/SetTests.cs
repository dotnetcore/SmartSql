using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class SetTests
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
            var set = new Set { ChildTags = new List<ITag>() };
            set.ChildTags.Add(new SqlText("Name = @Name", "@"));

            var context = CreateContext();

            var result = set.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NoChildMatches()
        {
            var set = new Set { ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            set.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "");

            var result = set.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_PrependSet_When_BuildingSql()
        {
            var set = new Set { ChildTags = new List<ITag>() };
            set.ChildTags.Add(new SqlText("Name = @Name", "@"));

            var context = CreateContext();

            set.BuildSql(context);

            var sql = context.SqlBuilder.ToString();
            sql.Should().Contain("Set");
            sql.Should().Contain("Name = @Name");
        }

        [Fact]
        public void Should_NotBuildSql_When_NoChildMatches()
        {
            var set = new Set { ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            set.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "");

            set.BuildSql(context);

            context.SqlBuilder.ToString().Should().BeEmpty();
        }

        [Fact]
        public void Should_HavePrependSetToSet()
        {
            var set = new Set();

            set.Prepend.Should().Be("Set");
        }
    }
}
