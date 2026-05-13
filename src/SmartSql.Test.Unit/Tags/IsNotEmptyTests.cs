using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsNotEmptyTests
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
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var tag = new IsNotEmpty { Property = "Name" };
            var context = CreateContext("Name", null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsEmptyString()
        {
            var tag = new IsNotEmpty { Property = "Name" };
            var context = CreateContext("Name", "");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyIsNonEmptyString()
        {
            var tag = new IsNotEmpty { Property = "Name" };
            var context = CreateContext("Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsEmptyCollection()
        {
            var tag = new IsNotEmpty { Property = "Items" };
            var context = CreateContext("Items", new List<string>());

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyIsNonEmptyCollection()
        {
            var tag = new IsNotEmpty { Property = "Items" };
            var context = CreateContext("Items", new List<string> { "a", "b" });

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsEmptyArray()
        {
            var tag = new IsNotEmpty { Property = "Ids" };
            var context = CreateContext("Ids", new long[0]);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyIsNonEmptyArray()
        {
            var tag = new IsNotEmpty { Property = "Ids" };
            var context = CreateContext("Ids", new long[] { 1, 2 });

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }
    }
}
