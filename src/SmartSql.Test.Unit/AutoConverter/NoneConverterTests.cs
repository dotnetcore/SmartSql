using System.Collections.Generic;
using FluentAssertions;
using SmartSql.AutoConverter;
using Xunit;

namespace SmartSql.Test.Unit.AutoConverter
{
    public class NoneConverterTests
    {
        [Fact]
        public void Should_JoinAllWords_When_ConvertCalled()
        {
            var converter = new NoneConverter();

            var result = converter.Convert(new[] { "Hello", "World" });

            result.Should().Be("HelloWorld");
        }

        [Fact]
        public void Should_ReturnEmptyString_When_NoWordsProvided()
        {
            var converter = new NoneConverter();

            var result = converter.Convert(new string[0]);

            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnSingleWord_When_OneWordProvided()
        {
            var converter = new NoneConverter();

            var result = converter.Convert(new[] { "Test" });

            result.Should().Be("Test");
        }

        [Fact]
        public void Should_NotThrow_When_InitializeCalledWithParameters()
        {
            var converter = new NoneConverter();

            var act = () => converter.Initialize(new Dictionary<string, object> { { "key", "value" } });

            act.Should().NotThrow();
        }

        [Fact]
        public void Should_NotThrow_When_InitializeCalledWithNull()
        {
            var converter = new NoneConverter();

            var act = () => converter.Initialize(null);

            act.Should().NotThrow();
        }
    }
}
