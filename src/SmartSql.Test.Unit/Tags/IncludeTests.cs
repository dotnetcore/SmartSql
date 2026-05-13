using System.Collections.Generic;
using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IncludeTests
    {
        private RequestContext CreateContext()
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
        public void Should_ReturnTrue_When_ChildTagMatches()
        {
            var tag = new Include { ChildTags = new List<ITag>() };
            tag.ChildTags.Add(new SqlText("SELECT *", "$"));

            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NoChildren()
        {
            var tag = new Include { ChildTags = new List<ITag>() };

            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_Throw_When_RequiredAndNoMatch()
        {
            var tag = new Include
            {
                Required = true,
                ChildTags = new List<ITag>(),
                Statement = new Statement
                {
                    Id = "GetEntity",
                    SqlMap = new SqlMap { Scope = "TestScope" }
                }
            };
            tag.ChildTags.Add(new IsNotEmpty { Property = "MissingProp" });

            var context = CreateContext();

            var act = () => tag.IsCondition(context);

            act.Should().Throw<TagRequiredFailException>();
        }

        [Fact]
        public void Should_ReturnTrue_When_MultipleChildrenAndFirstMatches()
        {
            var tag = new Include { ChildTags = new List<ITag>() };
            tag.ChildTags.Add(new SqlText("SELECT 1", "$"));
            tag.ChildTags.Add(new SqlText("SELECT 2", "$"));

            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_MultipleChildrenAndLaterMatches()
        {
            var tag = new Include { ChildTags = new List<ITag>() };
            tag.ChildTags.Add(new IsNotEmpty { Property = "Missing" });
            tag.ChildTags.Add(new SqlText("SELECT 1", "$"));

            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_RequiredFalseAndNoMatch()
        {
            var tag = new Include
            {
                Required = false,
                ChildTags = new List<ITag>()
            };
            tag.ChildTags.Add(new IsNotEmpty { Property = "MissingProp" });

            var context = CreateContext();

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }
    }
}
