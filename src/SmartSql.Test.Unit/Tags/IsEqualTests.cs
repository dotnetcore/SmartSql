using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsEqualTests
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
        public void Should_ReturnTrue_When_PropertyEqualsCompareValue()
        {
            var tag = new IsEqual { Property = "Status", CompareValue = "Active" };
            var context = CreateContext("Status", "Active");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyNotEqualsCompareValue()
        {
            var tag = new IsEqual { Property = "Status", CompareValue = "Active" };
            var context = CreateContext("Status", "Inactive");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new IsEqual { Property = "Status", CompareValue = "Active" };
            var context = CreateContext("Status", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_IntegerPropertyEqualsCompareValue()
        {
            var tag = new IsEqual { Property = "Type", CompareValue = "1" };
            var context = CreateContext("Type", 1);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_IntegerPropertyNotEqualsCompareValue()
        {
            var tag = new IsEqual { Property = "Type", CompareValue = "2" };
            var context = CreateContext("Type", 1);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }
    }
}
