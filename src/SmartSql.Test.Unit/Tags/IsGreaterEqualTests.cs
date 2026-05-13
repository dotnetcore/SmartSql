using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsGreaterEqualTests
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
        public void Should_ReturnTrue_When_PropertyGreaterThanCompareValue()
        {
            var tag = new IsGreaterEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", 25);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyEqualsCompareValue()
        {
            var tag = new IsGreaterEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", 18);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyLessThanCompareValue()
        {
            var tag = new IsGreaterEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", 10);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new IsGreaterEqual { Property = "Age", CompareValue = 18m };
            var context = CreateContext("Age", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_DecimalPropertyGreaterEqualCompareValue()
        {
            var tag = new IsGreaterEqual { Property = "Price", CompareValue = 99.9m };
            var context = CreateContext("Price", 100.5m);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_StringNumericPropertyGreaterEqualCompareValue()
        {
            var tag = new IsGreaterEqual { Property = "Count", CompareValue = 5m };
            var context = CreateContext("Count", "10");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNonNumericString()
        {
            var tag = new IsGreaterEqual { Property = "Value", CompareValue = 5m };
            var context = CreateContext("Value", "abc");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }
    }
}
