using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class DynamicTests
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
        public void Should_ReturnTrue_When_AnyChildMatches()
        {
            var dynamic = new Dynamic { ChildTags = new List<ITag>() };
            dynamic.ChildTags.Add(new SqlText("Name = @Name", "@"));

            var context = CreateContext();

            var result = dynamic.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NoChildMatches()
        {
            var dynamic = new Dynamic { ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            dynamic.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "");

            var result = dynamic.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_PrependCustomText_When_BuildingSql()
        {
            var dynamic = new Dynamic { Prepend = "AND", ChildTags = new List<ITag>() };
            dynamic.ChildTags.Add(new SqlText("Name = @Name", "@"));

            var context = CreateContext();

            dynamic.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("AND");
            context.SqlBuilder.ToString().Should().Contain("Name = @Name");
        }

        [Fact]
        public void Should_NotBuildSql_When_NoChildMatches()
        {
            var dynamic = new Dynamic { Prepend = "AND", ChildTags = new List<ITag>() };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            dynamic.ChildTags.Add(isNotEmpty);

            var context = CreateContext("Name", "");

            dynamic.BuildSql(context);

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
            var dynamic = new Dynamic { Min = 2, ChildTags = new List<ITag>(), Statement = statement };
            var isNotEmpty = new IsNotEmpty { Property = "Name" };
            dynamic.ChildTags.Add(isNotEmpty);
            dynamic.ChildTags.Add(new IsNotEmpty { Property = "Title" });

            var context = CreateContext("Name", "has-value");

            var act = () => dynamic.IsCondition(context);

            act.Should().Throw<TagMinMatchedFailException>();
        }

        [Fact]
        public void Should_ReturnTrue_When_MultipleChildrenAndSomeMatch()
        {
            var dynamic = new Dynamic { ChildTags = new List<ITag>() };
            dynamic.ChildTags.Add(new IsNotEmpty { Property = "Name" });
            dynamic.ChildTags.Add(new SqlText("Id = @Id", "@"));

            var context = CreateContext("Name", "");

            var result = dynamic.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_BuildOnlyMatchingChildren_When_SomeMatch()
        {
            var dynamic = new Dynamic { Prepend = "AND", ChildTags = new List<ITag>() };
            dynamic.ChildTags.Add(new IsNotEmpty { Property = "Name" });
            dynamic.ChildTags.Add(new SqlText("Id = @Id", "@"));

            var context = CreateContext("Name", "");

            dynamic.BuildSql(context);

            context.SqlBuilder.ToString().Should().Contain("Id = @Id");
            context.SqlBuilder.ToString().Should().Contain("AND");
        }
    }
}
