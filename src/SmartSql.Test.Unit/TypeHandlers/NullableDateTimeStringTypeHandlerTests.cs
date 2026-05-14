using System;
using FluentAssertions;
using SmartSql.Exceptions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class NullableDateTimeStringTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnNull_When_ValueIsNull()
        {
            var typeHandler = new NullableDateTimeStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.OfNull();

            var actual = typeHandler.GetValue(reader, 0, typeof(DateTime?));

            actual.Should().BeNull();
        }

        [Fact]
        public void Should_ReturnDateTime_When_StringIsValidDate()
        {
            var typeHandler = new NullableDateTimeStringTypeHandler();
            var expected = new DateTime(2024, 1, 15, 10, 30, 0);
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("2024-01-15 10:30:00");

            var actual = typeHandler.GetValue(reader, 0, typeof(DateTime?));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_ThrowSmartSqlException_When_StringIsInvalidDate()
        {
            var typeHandler = new NullableDateTimeStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("not-a-date");

            var act = () => typeHandler.GetValue(reader, 0, typeof(DateTime?));

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*not-a-date*");
        }

        [Fact]
        public void Should_IncludeColumnName_When_ThrowingForInvalidDate()
        {
            var typeHandler = new NullableDateTimeStringTypeHandler();
            var reader = EnhancedMockTypeHandlerDbDataReader.Of("bad-date", "UpdatedDate");

            var act = () => typeHandler.GetValue(reader, 0, typeof(DateTime?));

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*UpdatedDate*");
        }

        [Fact]
        public void Should_ReturnDBNull_When_ParameterValueIsNull()
        {
            var typeHandler = new NullableDateTimeStringTypeHandler();

            var result = typeHandler.GetSetParameterValue(null);

            result.Should().Be(DBNull.Value);
        }

        [Fact]
        public void Should_ReturnString_When_ParameterValueIsDateTime()
        {
            var typeHandler = new NullableDateTimeStringTypeHandler();
            var dateTime = new DateTime(2024, 6, 15, 12, 0, 0);

            var result = typeHandler.GetSetParameterValue(dateTime);

            result.Should().Be(dateTime.ToString());
        }
    }
}
