using FluentAssertions;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class NullableBooleanStringTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnNull_When_ValueIsNull()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnTrue_When_StringIsOne()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("1");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_StringIsZero()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("0");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_StringIsTrue()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("True");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_StringIsFalse()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("False");

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeFalse();
        }

        [Fact]
        public void Should_ThrowSmartSqlException_When_StringIsInvalid()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("invalid");

            var act = () => typeHandler.GetValue(reader, 0, typeof(bool?));

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*invalid*");
        }

        [Fact]
        public void Should_ReturnDBNull_When_ParameterValueIsNull()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();

            var result = typeHandler.GetSetParameterValue(null);

            result.Should().Be(System.DBNull.Value);
        }

        [Fact]
        public void Should_ReturnStringOne_When_ParameterValueIsTrue()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();

            var result = typeHandler.GetSetParameterValue(true);

            result.Should().Be("1");
        }

        [Fact]
        public void Should_ReturnStringZero_When_ParameterValueIsFalse()
        {
            var typeHandler = new NullableBooleanStringTypeHandler();

            var result = typeHandler.GetSetParameterValue(false);

            result.Should().Be("0");
        }
    }
}
