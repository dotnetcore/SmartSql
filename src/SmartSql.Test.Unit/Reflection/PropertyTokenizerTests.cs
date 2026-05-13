using FluentAssertions;
using SmartSql.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace SmartSql.Test.Unit.Reflection
{
    public class PropertyTokenizerTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public PropertyTokenizerTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void Should_ParsePropertyName_When_NoChildren()
        {
            var propertyTokenizer = new PropertyTokenizer("User");

            var current = propertyTokenizer.Current;
            current.Name.Should().Be("User");
            current.Index.Should().BeNull();
            current.Children.Should().BeNull();

            propertyTokenizer.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void Should_ParseNestedProperty_When_OneLevelNesting()
        {
            var propertyTokenizer = new PropertyTokenizer("User.Name");

            var current = propertyTokenizer.Current;
            current.Name.Should().Be("User");
            current.Index.Should().BeNull();
            current.Children.Should().Be("Name");

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            current.Name.Should().Be("Name");
            current.Index.Should().BeNull();
            current.Children.Should().BeNull();

            propertyTokenizer.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void Should_ParseNestedProperty_When_TwoLevelNesting()
        {
            var propertyTokenizer = new PropertyTokenizer("User.Info.Id");

            var current = propertyTokenizer.Current;
            current.Name.Should().Be("User");
            current.Index.Should().BeNull();
            current.Children.Should().Be("Info.Id");

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            current.Name.Should().Be("Info");
            current.Index.Should().BeNull();
            current.Children.Should().Be("Id");

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            current.Name.Should().Be("Id");
            current.Index.Should().BeNull();
            current.Children.Should().BeNull();

            propertyTokenizer.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void Should_ParseIndexAccess_When_PropertyHasNumericIndex()
        {
            var propertyTokenizer = new PropertyTokenizer("Items[0]");

            var current = propertyTokenizer.Current;
            current.Name.Should().Be("Items");
            current.Index.Should().Be("0");
            current.Children.Should().BeNull();

            propertyTokenizer.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void Should_ParseIndexAccess_When_NestedPropertyHasNumericIndex()
        {
            var propertyTokenizer = new PropertyTokenizer("Order.Items[0]");

            var current = propertyTokenizer.Current;
            current.Name.Should().Be("Order");
            current.Index.Should().BeNull();
            current.Children.Should().Be("Items[0]");

            propertyTokenizer.MoveNext();
            current = propertyTokenizer.Current;
            current.Name.Should().Be("Items");
            current.Index.Should().Be("0");
            current.Children.Should().BeNull();

            propertyTokenizer.MoveNext().Should().BeFalse();
        }

        [Fact]
        public void Should_ParseIndexAccess_When_PropertyHasStringIndex()
        {
            var propertyTokenizer = new PropertyTokenizer("Items[Name]");

            var current = propertyTokenizer.Current;
            current.Name.Should().Be("Items");
            current.Index.Should().Be("Name");
            current.Children.Should().BeNull();
        }
    }
}
