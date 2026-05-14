using FluentAssertions;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class BooleanStringTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnTrue_When_StringIsOne()
        {
            var typeHandler = new BooleanStringTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("1");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool));

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_StringIsZero()
        {
            var typeHandler = new BooleanStringTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("0");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool));

            actual.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_StringIsTrue()
        {
            var typeHandler = new BooleanStringTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("True");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool));

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_StringIsFalse()
        {
            var typeHandler = new BooleanStringTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("False");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool));

            actual.Should().BeFalse();
        }

        [Fact]
        public void Should_ThrowSmartSqlException_When_StringIsInvalid()
        {
            var typeHandler = new BooleanStringTypeHandler();
            var reader = MockTypeHandlerDbDataReader.Of("invalid");

            var act = () => typeHandler.GetValue(reader, 0, typeof(bool));

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*invalid*");
        }

        [Fact]
        public void Should_ReturnStringOne_When_ParameterValueIsTrue()
        {
            var typeHandler = new BooleanStringTypeHandler();

            var result = typeHandler.GetSetParameterValue(true);

            result.Should().Be("1");
        }

        [Fact]
        public void Should_ReturnStringZero_When_ParameterValueIsFalse()
        {
            var typeHandler = new BooleanStringTypeHandler();

            var result = typeHandler.GetSetParameterValue(false);

            result.Should().Be("0");
        }
    }
}
