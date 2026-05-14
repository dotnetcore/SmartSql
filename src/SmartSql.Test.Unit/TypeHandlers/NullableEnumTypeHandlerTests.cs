using System;
using FluentAssertions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class NullableEnumTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnNull_When_ValueIsNull()
        {
            var typeHandler = new NullableEnumTypeHandler<DaysOfWeek?>();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var actual = typeHandler.GetValue(reader, 0, typeof(DaysOfWeek?));

            actual.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnEnumValue_When_ReadingIntFromDb()
        {
            var typeHandler = new NullableEnumTypeHandler<DaysOfWeek?>();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(2);

            var actual = typeHandler.GetValue(reader, 0, typeof(DaysOfWeek?));

            actual.Should().Be(DaysOfWeek.Tuesday);
        }

        [Fact]
        public void Should_ReturnFirstEnumValue_When_ReadingFirstIntFromDb()
        {
            var typeHandler = new NullableEnumTypeHandler<DaysOfWeek?>();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of(1);

            var actual = typeHandler.GetValue(reader, 0, typeof(DaysOfWeek?));

            actual.Should().Be(DaysOfWeek.Monday);
        }

        [Fact]
        public void Should_ReturnDBNull_When_ParameterValueIsNull()
        {
            var typeHandler = new NullableEnumTypeHandler<DaysOfWeek?>();

            var result = typeHandler.GetSetParameterValue(null);

            result.Should().Be(DBNull.Value);
        }

        [Fact]
        public void Should_ReturnUnderlyingInt_When_ParameterValueIsEnum()
        {
            var typeHandler = new NullableEnumTypeHandler<DaysOfWeek?>();

            var result = typeHandler.GetSetParameterValue(DaysOfWeek.Wednesday);

            result.Should().Be(3);
        }
    }
}
