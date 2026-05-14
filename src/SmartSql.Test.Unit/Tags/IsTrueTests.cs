using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsTrueTests
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
        public void Should_ReturnTrue_When_PropertyIsTrue()
        {
            var tag = new IsTrue { Property = "IsActive" };
            var context = CreateContext("IsActive", true);

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsFalse()
        {
            var tag = new IsTrue { Property = "IsActive" };
            var context = CreateContext("IsActive", false);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new IsTrue { Property = "IsActive" };
            var context = CreateContext("IsActive", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsString()
        {
            var tag = new IsTrue { Property = "IsActive" };
            var context = CreateContext("IsActive", "true");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsInteger()
        {
            var tag = new IsTrue { Property = "IsActive" };
            var context = CreateContext("IsActive", 1);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }
    }
}
