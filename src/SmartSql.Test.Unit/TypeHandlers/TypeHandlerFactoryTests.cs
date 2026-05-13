using FluentAssertions;
using SmartSql.Test.Unit.TestEntities;
using SmartSql.TypeHandlers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class TypeHandlerFactoryTests
    {
        private readonly TypeHandlerFactory _typeHandlerFactory = new TypeHandlerFactory();
        private readonly Type _enumType = typeof(NumericalEnum);

        [Fact]
        public void Should_ResolveEnumTypeHandler_When_EnumTypeRequested()
        {
            var typeHandler = _typeHandlerFactory.GetTypeHandler(_enumType);

            typeHandler.Should().BeOfType<EnumTypeHandler<NumericalEnum>>();
        }

        [Fact]
        public void Should_BeThreadSafe_When_ConcurrentTypeHandlerResolution()
        {
            var taskMax = 200;
            var current = 0;
            var tasks = new Task[taskMax];

            while (current < taskMax)
            {
                var task = new Task(() =>
                {
                    var typeHandler = _typeHandlerFactory.GetTypeHandler(_enumType);
                });
                tasks[current] = task;
                task.Start();
                current++;
            }

            Task.WaitAll(tasks);
        }

        [Fact]
        public void Should_ResolveStringTypeHandler_When_RequestingStringType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(string));
            handler.Should().BeOfType<StringTypeHandler>();
        }

        [Fact]
        public void Should_ResolveInt32TypeHandler_When_RequestingInt32Type()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(int));
            handler.Should().BeOfType<Int32TypeHandler>();
        }

        [Fact]
        public void Should_ResolveInt64TypeHandler_When_RequestingInt64Type()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(long));
            handler.Should().BeOfType<Int64TypeHandler>();
        }

        [Fact]
        public void Should_ResolveBooleanTypeHandler_When_RequestingBooleanType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(bool));
            handler.Should().BeOfType<BooleanTypeHandler>();
        }

        [Fact]
        public void Should_ResolveDateTimeTypeHandler_When_RequestingDateTimeType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(DateTime));
            handler.Should().BeOfType<DateTimeTypeHandler>();
        }

        [Fact]
        public void Should_ResolveGuidTypeHandler_When_RequestingGuidType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(Guid));
            handler.Should().BeOfType<GuidTypeHandler>();
        }

        [Fact]
        public void Should_ResolveDecimalTypeHandler_When_RequestingDecimalType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(decimal));
            handler.Should().BeOfType<DecimalTypeHandler>();
        }

        [Fact]
        public void Should_ResolveDoubleTypeHandler_When_RequestingDoubleType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(double));
            handler.Should().BeOfType<DoubleTypeHandler>();
        }

        [Fact]
        public void Should_ResolveSingleTypeHandler_When_RequestingFloatType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(float));
            handler.Should().BeOfType<SingleTypeHandler>();
        }

        [Fact]
        public void Should_ResolveInt16TypeHandler_When_RequestingInt16Type()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(short));
            handler.Should().BeOfType<Int16TypeHandler>();
        }

        [Fact]
        public void Should_ResolveByteTypeHandler_When_RequestingByteType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(byte));
            handler.Should().BeOfType<ByteTypeHandler>();
        }

        [Fact]
        public void Should_ResolveByteArrayTypeHandler_When_RequestingByteArrayType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(byte[]));
            handler.Should().BeOfType<ByteArrayTypeHandler>();
        }

        [Fact]
        public void Should_ResolveCharArrayTypeHandler_When_RequestingCharArrayType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(char[]));
            handler.Should().BeOfType<CharArrayTypeHandler>();
        }

        [Fact]
        public void Should_ResolveTimeSpanTypeHandler_When_RequestingTimeSpanType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(TimeSpan));
            handler.Should().BeOfType<TimeSpanTypeHandler>();
        }

        [Fact]
        public void Should_ResolveNullableInt32AnyTypeHandler_When_RequestingNullableInt32Type()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(int?));
            handler.Should().BeOfType<NullableInt32AnyTypeHandler>();
        }

        [Fact]
        public void Should_ResolveNullableBooleanAnyTypeHandler_When_RequestingNullableBooleanType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(bool?));
            handler.Should().BeOfType<NullableBooleanAnyTypeHandler>();
        }

        [Fact]
        public void Should_ResolveNullableDateTimeAnyTypeHandler_When_RequestingNullableDateTimeType()
        {
            var handler = _typeHandlerFactory.GetTypeHandler(typeof(DateTime?));
            handler.Should().BeOfType<NullableDateTimeAnyTypeHandler>();
        }
    }
}
