using FluentAssertions;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class NullableBooleanCharTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnNull_When_ValueIsNull()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnTrue_When_CharIsOne()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of('1');

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_CharIsZero()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of('0');

            var actual = typeHandler.GetValue(reader, 0, typeof(bool?));

            actual.Should().BeFalse();
        }

        [Fact]
        public void Should_ThrowSmartSqlException_When_CharIsInvalid()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of('X');

            var act = () => typeHandler.GetValue(reader, 0, typeof(bool?));

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*X*");
        }

        [Fact]
        public void Should_ReturnDBNull_When_ParameterValueIsNull()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();

            var result = typeHandler.GetSetParameterValue(null);

            result.Should().Be(System.DBNull.Value);
        }

        [Fact]
        public void Should_ReturnCharOne_When_ParameterValueIsTrue()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();

            var result = typeHandler.GetSetParameterValue(true);

            result.Should().Be('1');
        }

        [Fact]
        public void Should_ReturnCharZero_When_ParameterValueIsFalse()
        {
            var typeHandler = new NullableBooleanCharTypeHandler();

            var result = typeHandler.GetSetParameterValue(false);

            result.Should().Be('0');
        }
    }
}
