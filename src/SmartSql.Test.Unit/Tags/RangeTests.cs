using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class RangeTests
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
        public void Should_ReturnTrue_When_ValueIsInRange()
        {
            var tag = new Range { Property = "Age", Min = 18, Max = 65 };
            var context = CreateContext("Age", 30);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ValueIsAtMin()
        {
            var tag = new Range { Property = "Age", Min = 18, Max = 65 };
            var context = CreateContext("Age", 18);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ValueIsAtMax()
        {
            var tag = new Range { Property = "Age", Min = 18, Max = 65 };
            var context = CreateContext("Age", 65);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_ValueIsBelowMin()
        {
            var tag = new Range { Property = "Age", Min = 18, Max = 65 };
            var context = CreateContext("Age", 17);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_ValueIsAboveMax()
        {
            var tag = new Range { Property = "Age", Min = 18, Max = 65 };
            var context = CreateContext("Age", 66);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_DecimalValueIsInRange()
        {
            var tag = new Range { Property = "Price", Min = 10.5m, Max = 99.9m };
            var context = CreateContext("Price", 50.0m);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_IntegerValueMatchesMinMax()
        {
            var tag = new Range { Property = "Score", Min = 100, Max = 100 };
            var context = CreateContext("Score", 100);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_StringValueConvertsToInRange()
        {
            var tag = new Range { Property = "Age", Min = 18, Max = 65 };
            var context = CreateContext("Age", "25");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }
    }
}
