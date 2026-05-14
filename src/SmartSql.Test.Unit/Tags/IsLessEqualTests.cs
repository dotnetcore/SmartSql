using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsLessEqualTests
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
        public void Should_ReturnTrue_When_PropertyLessThanCompareValue()
        {
            var tag = new IsLessEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", 10);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyEqualsCompareValue()
        {
            var tag = new IsLessEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", 18);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyGreaterThanCompareValue()
        {
            var tag = new IsLessEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", 25);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new IsLessEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_DecimalPropertyLessEqualCompareValue()
        {
            var tag = new IsLessEqual { Property = "Price", CompareValue = 99.9m };
            var context = CreateContext("Price", 50.5m);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_StringNumericPropertyLessEqualCompareValue()
        {
            var tag = new IsLessEqual { Property = "Count", CompareValue = 10m };
            var context = CreateContext("Count", "5");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNonNumericString()
        {
            var tag = new IsLessEqual { Property = "Value", CompareValue = 5m };
            var context = CreateContext("Value", "abc");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }
    }
}
