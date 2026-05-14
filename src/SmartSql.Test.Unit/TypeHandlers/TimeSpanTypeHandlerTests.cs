using System;
using FluentAssertions;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class TimeSpanTypeHandlerTests
    {
        [Fact]
        public void Should_ReturnTimeSpan_When_ReadingTimeSpanValue()
        {
            var handler = new TimeSpanTypeHandler();
            var expected = new TimeSpan(1, 2, 3);

            var actual = handler.GetValue(MockTypeHandlerDbDataReader.Of(expected), 0, typeof(TimeSpan));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnTimeSpan_When_ReadingTimeSpanFromTimeSpan()
        {
            var handler = new TimeSpanAnyTypeHandler();
            var expected = new TimeSpan(1, 2, 3);

            var actual = handler.GetValue(MockTypeHandlerDbDataReader.Of(expected), 0, typeof(TimeSpan));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnTimeSpan_When_ReadingTimeSpanFromInt64()
        {
            var handler = new TimeSpanAnyTypeHandler();
            var expected = new TimeSpan(1, 2, 3);

            var actual = handler.GetValue(MockTypeHandlerDbDataReader.Of(expected.Ticks), 0, typeof(TimeSpan));

            actual.Should().Be(expected);
        }

        [Fact]
        public void Should_ReturnTimeSpan_When_ReadingTimeSpanFromString()
        {
            var handler = new TimeSpanAnyTypeHandler();

            var actual = handler.GetValue(MockTypeHandlerDbDataReader.Of("00:01:02"), 0, typeof(TimeSpan));

            actual.Should().Be(new TimeSpan(0, 1, 2));
        }

        [Fact]
        public void Should_ReturnTimeSpan_When_ReadingTimeSpanFromInt32()
        {
            var handler = new TimeSpanAnyTypeHandler();
            var ticks = (int)new TimeSpan(0, 1, 2).Ticks;

            var actual = handler.GetValue(MockTypeHandlerDbDataReader.Of(ticks), 0, typeof(TimeSpan));

            actual.Should().Be(new TimeSpan(ticks));
        }
    }
}
