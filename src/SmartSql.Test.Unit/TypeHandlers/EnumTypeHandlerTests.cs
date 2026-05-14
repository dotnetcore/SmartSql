using FluentAssertions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public enum DaysOfWeek
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3
    }

    public class EnumTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnEnumValue_When_ReadingIntFromDb()
        {
            var typeHandler = new EnumTypeHandler<DaysOfWeek>();
            var reader = MockTypeHandlerDbDataReader.Of(2);

            var actual = typeHandler.GetValue(reader, 0, typeof(DaysOfWeek));

            actual.Should().Be(DaysOfWeek.Tuesday);
        }

        [Fact]
        public void Should_ReturnFirstEnumValue_When_ReadingFirstIntFromDb()
        {
            var typeHandler = new EnumTypeHandler<DaysOfWeek>();
            var reader = MockTypeHandlerDbDataReader.Of(1);

            var actual = typeHandler.GetValue(reader, 0, typeof(DaysOfWeek));

            actual.Should().Be(DaysOfWeek.Monday);
        }

        [Fact]
        public void Should_ReturnUnderlyingInt_When_ParameterValueIsEnum()
        {
            var typeHandler = new EnumTypeHandler<DaysOfWeek>();

            var result = typeHandler.GetSetParameterValue(DaysOfWeek.Wednesday);

            result.Should().Be(3);
        }

        [Fact]
        public void Should_ReturnUnderlyingInt_When_ParameterValueIsFirstEnum()
        {
            var typeHandler = new EnumTypeHandler<DaysOfWeek>();

            var result = typeHandler.GetSetParameterValue(DaysOfWeek.Monday);

            result.Should().Be(1);
        }
    }
}
