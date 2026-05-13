using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class SwitchTests
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
        public void Should_MatchCase_When_PropertyEqualsCompareValue()
        {
            var tag = new Switch { Property = "Status" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Case { Property = "Status", CompareValue = "Active" },
                new Switch.Default()
            };

            var context = CreateContext("Status", "Active");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_MatchDefault_When_NoCaseMatches()
        {
            var tag = new Switch { Property = "Status" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Case { Property = "Status", CompareValue = "Active" },
                new Switch.Default()
            };

            var context = CreateContext("Status", "Inactive");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_NoCaseOrDefault()
        {
            var tag = new Switch { Property = "Status" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Case { Property = "Status", CompareValue = "Active" }
            };

            var context = CreateContext("Status", "Inactive");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_MatchFirstCase_When_MultipleCasesMatch()
        {
            var tag = new Switch { Property = "Type" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Case { Property = "Type", CompareValue = "1" },
                new Switch.Case { Property = "Type", CompareValue = "1" },
                new Switch.Default()
            };

            var context = CreateContext("Type", 1);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_MatchSecondCase_When_FirstDoesNotMatch()
        {
            var tag = new Switch { Property = "Status" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Case { Property = "Status", CompareValue = "Active" },
                new Switch.Case { Property = "Status", CompareValue = "Pending" }
            };

            var context = CreateContext("Status", "Pending");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new Switch { Property = "Status" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Case { Property = "Status", CompareValue = "Active" }
            };

            var context = CreateContext("Status", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_OnlyDefaultPresent()
        {
            var tag = new Switch { Property = "Status" };
            tag.ChildTags = new List<ITag>
            {
                new Switch.Default()
            };

            var context = CreateContext("Status", "Anything");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }
    }
}
