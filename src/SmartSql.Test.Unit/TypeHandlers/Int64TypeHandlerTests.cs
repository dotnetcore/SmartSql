using FluentAssertions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class Int64TypeHandlerTests
    {
        [Fact]
        public void Should_ReturnInt64_When_ReadingInt64Value()
        {
            var typeHandler = new Int64TypeHandler();
            long expected = 1L;

            var actual = typeHandler.GetValue(MockTypeHandlerDbDataReader.Of(expected), 1, typeof(long));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnByte_When_ReadingByteAsInt64()
        {
            var typeHandler = new Int64ByteTypeHandler();
            byte expected = 1;

            var actual = typeHandler.GetValue(MockTypeHandlerDbDataReader.Of(expected), 1, typeof(byte));

            actual.Should().Be(expected);
        }
    }
}
