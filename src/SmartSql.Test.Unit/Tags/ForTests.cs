using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class ForTests
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
        public void Should_ReturnTrue_When_CollectionIsNonEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new List<int> { 1, 2, 3 });

            var result = forTag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_CollectionIsEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new List<int>());

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNull()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", null);

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsNotEnumerable()
        {
            var forTag = new For { Property = "Id", Key = "Id" };
            var context = CreateContext("Id", 42);

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_ArrayIsNonEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new[] { 1, 2, 3 });

            var result = forTag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_ArrayIsEmpty()
        {
            var forTag = new For { Property = "Ids", Key = "Id" };
            var context = CreateContext("Ids", new int[0]);

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyIsMissing()
        {
            var forTag = new For { Property = "Ids", Key = "Id", Required = false };
            var sqlParams = new SqlParameterCollection();
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();

            var result = forTag.IsCondition(context);

            result.Should().BeFalse();
        }
    }
}
