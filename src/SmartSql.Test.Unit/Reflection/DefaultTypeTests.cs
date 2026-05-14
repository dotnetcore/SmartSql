using FluentAssertions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class DefaultTypeTests
    {
        [Fact]
        public void Should_ReturnZero_When_DefaultIntRequested()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(int)).GetValue(null);

            defaultVal.Should().Be(0);
        }

        [Fact]
        public void Should_ReturnZero_When_DefaultDecimalRequested()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(decimal)).GetValue(null);

            defaultVal.Should().Be(0M);
        }

        [Fact]
        public void Should_ReturnNull_When_DefaultStringRequested()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(string)).GetValue(null);

            defaultVal.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnDefaultEnumValue_When_DefaultEnumRequested()
        {
            var defaultVal = DefaultType.GetDefaultField(typeof(UserStatus)).GetValue(null);

            defaultVal.Should().Be(default(UserStatus));
        }
    }
}
