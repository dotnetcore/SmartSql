using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.Deserializer;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.TypeHandlers;
using Xunit;
using ResultEntry = SmartSql.Configuration.Result;

namespace SmartSql.Test.Unit.Deserializer;

public class AsyncDeserializerTests
{
    #region ValueTypeDeserializer async tests

    public class ValueTypeDeserializerAsyncTests
    {
        private readonly ValueTypeDeserializer _sut = new();
        private readonly SmartSqlConfig _config = new();

        private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader)
        {
            var wrapper = new DataReaderWrapper(mockReader.Object);
            return new ExecutionContext
            {
                SmartSqlConfig = _config,
                DataReaderWrapper = wrapper,
                Request = new RequestContext(),
                Result = new SingleResultContext<T>()
            };
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnValue_When_DataReaderHasRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(true);
            mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            mockReader.Setup(r => r.GetValue(0)).Returns(42);

            var context = CreateContext<int>(mockReader);

            var result = await _sut.ToSingleAsync<int>(context);

            result.Should().Be(42);
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnDefault_When_NoRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(false);

            var context = CreateContext<int>(mockReader);

            var result = await _sut.ToSingleAsync<int>(context);

            result.Should().Be(default(int));
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnString_When_DataReaderHasStrings()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(true);
            mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            mockReader.Setup(r => r.GetValue(0)).Returns("hello-async");

            var context = CreateContext<string>(mockReader);

            var result = await _sut.ToSingleAsync<string>(context);

            result.Should().Be("hello-async");
        }

        [Fact]
        public async Task ToListAsync_Should_ReturnAllValues_When_DataReaderHasMultipleRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(true);

            var readCount = 0;
            mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    readCount++;
                    return readCount <= 2;
                });

            var valueCallCount = 0;
            mockReader.Setup(r => r.GetValue(0))
                .Returns(() =>
                {
                    valueCallCount++;
                    return valueCallCount * 10;
                });

            var context = CreateContext<int>(mockReader);

            var result = await _sut.ToListAsync<int>(context);

            result.Should().HaveCount(2);
            result[0].Should().Be(10);
            result[1].Should().Be(20);
        }

        [Fact]
        public async Task ToListAsync_Should_ReturnEmptyList_When_NoRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(false);

            var context = CreateContext<int>(mockReader);

            var result = await _sut.ToListAsync<int>(context);

            result.Should().BeEmpty();
        }
    }

    #endregion

    #region DynamicDeserializer async tests

    public class DynamicDeserializerAsyncTests
    {
        private readonly DynamicDeserializer _sut = new();
        private readonly SmartSqlConfig _config = new();

        private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader)
        {
            var wrapper = new DataReaderWrapper(mockReader.Object);
            return new ExecutionContext
            {
                SmartSqlConfig = _config,
                DataReaderWrapper = wrapper,
                Request = new RequestContext(),
                Result = new SingleResultContext<T>()
            };
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnDynamicRow_When_DataReaderHasRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(true);
            mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            mockReader.Setup(r => r.FieldCount).Returns(2);
            mockReader.Setup(r => r.GetName(0)).Returns("Id");
            mockReader.Setup(r => r.GetName(1)).Returns("Name");
            mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                .Callback<object[]>(values =>
                {
                    values[0] = 1L;
                    values[1] = "AsyncTest";
                });

            var context = CreateContext<DynamicRow>(mockReader);

            var result = await _sut.ToSingleAsync<DynamicRow>(context);

            result.Should().NotBeNull();
            result["Id"].Should().Be(1L);
            result["Name"].Should().Be("AsyncTest");
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnDefault_When_NoRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(false);

            var context = CreateContext<DynamicRow>(mockReader);

            var result = await _sut.ToSingleAsync<DynamicRow>(context);

            result.Should().BeNull();
        }

        [Fact]
        public async Task ToListAsync_Should_ReturnAllRows_When_DataReaderHasMultipleRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(true);

            var readCount = 0;
            mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    readCount++;
                    return readCount <= 2;
                });

            mockReader.Setup(r => r.FieldCount).Returns(2);
            mockReader.Setup(r => r.GetName(0)).Returns("Id");
            mockReader.Setup(r => r.GetName(1)).Returns("Name");

            var valueCallCount = 0;
            mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                .Callback<object[]>(values =>
                {
                    valueCallCount++;
                    values[0] = valueCallCount;
                    values[1] = $"Async{valueCallCount}";
                });

            var context = CreateContext<DynamicRow>(mockReader);

            var result = await _sut.ToListAsync<DynamicRow>(context);

            result.Should().HaveCount(2);
            result[0]["Id"].Should().Be(1);
            result[1]["Id"].Should().Be(2);
        }

        [Fact]
        public async Task ToListAsync_Should_ReturnEmptyList_When_NoRows()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(false);

            var context = CreateContext<DynamicRow>(mockReader);

            var result = await _sut.ToListAsync<DynamicRow>(context);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnAsObject_When_ResultTypeIsObject()
        {
            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.HasRows).Returns(true);
            mockReader.Setup(r => r.ReadAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            mockReader.Setup(r => r.FieldCount).Returns(1);
            mockReader.Setup(r => r.GetName(0)).Returns("Value");
            mockReader.Setup(r => r.GetValues(It.IsAny<object[]>()))
                .Callback<object[]>(values => { values[0] = 42; });

            var context = CreateContext<object>(mockReader);

            var result = await _sut.ToSingleAsync<object>(context);

            result.Should().NotBeNull();
            result.Should().BeOfType<DynamicRow>();
        }
    }

    #endregion

    #region ValueTupleDeserializer async tests

    public class ValueTupleDeserializerAsyncTests
    {
        private readonly Mock<IDeserializerFactory> _mockFactory;
        private readonly ValueTupleDeserializer _sut;

        public ValueTupleDeserializerAsyncTests()
        {
            _mockFactory = new Mock<IDeserializerFactory>();
            _sut = new ValueTupleDeserializer(_mockFactory.Object);
        }

        private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader)
        {
            var wrapper = new DataReaderWrapper(mockReader.Object);
            return new ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig(),
                DataReaderWrapper = wrapper,
                Request = new RequestContext(),
                Result = new SingleResultContext<T>()
            };
        }

        [Fact]
        public void CanDeserialize_Should_ReturnTrue_When_TypeIsValueTuple()
        {
            var context = CreateContext<(int, string)>(new Mock<DbDataReader>());

            _sut.CanDeserialize(context, typeof((int, string))).Should().BeTrue();
        }

        [Fact]
        public void CanDeserialize_Should_ReturnFalse_When_TypeIsNotValueTuple()
        {
            var context = CreateContext<int>(new Mock<DbDataReader>());

            _sut.CanDeserialize(context, typeof(int)).Should().BeFalse();
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnTuple_When_DataReaderHasMultipleResults()
        {
            var mockIntDeserializer = new Mock<IDataReaderDeserializer>();
            mockIntDeserializer.Setup(d => d.ToSingle<int>(It.IsAny<ExecutionContext>())).Returns(42);

            var mockStringDeserializer = new Mock<IDataReaderDeserializer>();
            mockStringDeserializer.Setup(d => d.ToSingle<string>(It.IsAny<ExecutionContext>())).Returns("test");

            var callCount = 0;
            _mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns<ExecutionContext, Type, bool>((ctx, type, isMultiple) =>
                {
                    callCount++;
                    return callCount == 1 ? mockIntDeserializer.Object : mockStringDeserializer.Object;
                });

            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.NextResultAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var context = CreateContext<(int, string)>(mockReader);

            var result = await _sut.ToSingleAsync<(int, string)>(context);

            result.Item1.Should().Be(42);
            result.Item2.Should().Be("test");
        }

        [Fact]
        public async Task ToSingleAsync_Should_Break_When_NextResultReturnsFalse()
        {
            var mockIntDeserializer = new Mock<IDataReaderDeserializer>();
            mockIntDeserializer.Setup(d => d.ToSingle<int>(It.IsAny<ExecutionContext>())).Returns(1);

            var mockStringDeserializer = new Mock<IDataReaderDeserializer>();

            var callCount = 0;
            _mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns<ExecutionContext, Type, bool>((ctx, type, isMultiple) =>
                {
                    callCount++;
                    return callCount == 1 ? mockIntDeserializer.Object : mockStringDeserializer.Object;
                });

            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.NextResultAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var context = CreateContext<(int, string)>(mockReader);

            var result = await _sut.ToSingleAsync<(int, string)>(context);

            result.Item1.Should().Be(1);
            result.Item2.Should().BeNull();
        }

        [Fact]
        public void ToList_Should_Throw_When_Called()
        {
            var context = CreateContext<(int, string)>(new Mock<DbDataReader>());

            var act = () => _sut.ToList<(int, string)>(context);

            act.Should().Throw<SmartSqlException>()
                .WithMessage("MultipleResultDeserializer can not support ToList.");
        }

        [Fact]
        public async Task ToListAsync_Should_Throw_When_Called()
        {
            var context = CreateContext<(int, string)>(new Mock<DbDataReader>());

            var act = () => _sut.ToListAsync<(int, string)>(context);

            await act.Should().ThrowAsync<SmartSqlException>()
                .WithMessage("MultipleResultDeserializer can not support ToListAsync.");
        }
    }

    #endregion

    #region MultipleResultDeserializer async tests

    public class MultipleResultDeserializerAsyncTests
    {
        private readonly Mock<IDeserializerFactory> _mockFactory;
        private readonly MultipleResultDeserializer _sut;

        public MultipleResultDeserializerAsyncTests()
        {
            _mockFactory = new Mock<IDeserializerFactory>();
            _sut = new MultipleResultDeserializer(_mockFactory.Object);
        }

        private static void SetMultipleResultMap(AbstractRequestContext request, MultipleResultMap map)
        {
            var prop = typeof(AbstractRequestContext).GetProperty("MultipleResultMap",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(request, map);
        }

        private ExecutionContext CreateContext<T>(Mock<DbDataReader> mockReader, MultipleResultMap multipleResultMap = null)
        {
            var wrapper = new DataReaderWrapper(mockReader.Object);
            var request = new RequestContext();

            if (multipleResultMap != null)
            {
                SetMultipleResultMap(request, multipleResultMap);
            }

            return new ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig(),
                DataReaderWrapper = wrapper,
                Request = request,
                Result = new SingleResultContext<T>()
            };
        }

        [Fact]
        public void CanDeserialize_Should_ReturnTrue_When_IsMultipleAndNotValueTuple()
        {
            var context = CreateContext<TestEntity>(new Mock<DbDataReader>());

            _sut.CanDeserialize(context, typeof(TestEntity), isMultiple: true).Should().BeTrue();
        }

        [Fact]
        public void CanDeserialize_Should_ReturnFalse_When_NotMultiple()
        {
            var context = CreateContext<TestEntity>(new Mock<DbDataReader>());

            _sut.CanDeserialize(context, typeof(TestEntity), isMultiple: false).Should().BeFalse();
        }

        [Fact]
        public void CanDeserialize_Should_ReturnFalse_When_IsValueTuple()
        {
            var context = CreateContext<(int, string)>(new Mock<DbDataReader>());

            _sut.CanDeserialize(context, typeof((int, string)), isMultiple: true).Should().BeFalse();
        }

        [Fact]
        public async Task ToSingleAsync_Should_UseRootDeserializer_When_RootExists()
        {
            var mockRootDeserializer = new Mock<IDataReaderDeserializer>();
            var rootEntity = new TestEntity { Id = 1, Name = "Root" };
            mockRootDeserializer.Setup(d => d.ToSingle<TestEntity>(It.IsAny<ExecutionContext>()))
                .Returns(rootEntity);

            _mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns(mockRootDeserializer.Object);

            var rootEntry = new ResultEntry { Property = ResultEntry.ROOT_PROPERTY };
            var multipleResultMap = new MultipleResultMap
            {
                Root = rootEntry,
                Results = new List<ResultEntry>()
            };

            var mockReader = new Mock<DbDataReader>();
            mockReader.Setup(r => r.NextResultAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var context = CreateContext<TestEntity>(mockReader, multipleResultMap);

            var result = await _sut.ToSingleAsync<TestEntity>(context);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Root");
        }

        [Fact]
        public async Task ToSingleAsync_Should_ReturnDefault_When_RootDeserializerReturnsNull()
        {
            var mockRootDeserializer = new Mock<IDataReaderDeserializer>();
            mockRootDeserializer.Setup(d => d.ToSingle<TestEntity>(It.IsAny<ExecutionContext>()))
                .Returns((TestEntity)null);

            _mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), It.IsAny<Type>(), It.IsAny<bool>()))
                .Returns(mockRootDeserializer.Object);

            var rootEntry = new ResultEntry { Property = ResultEntry.ROOT_PROPERTY };
            var multipleResultMap = new MultipleResultMap
            {
                Root = rootEntry,
                Results = new List<ResultEntry>()
            };

            var mockReader = new Mock<DbDataReader>();

            var context = CreateContext<TestEntity>(mockReader, multipleResultMap);

            var result = await _sut.ToSingleAsync<TestEntity>(context);

            result.Should().BeNull();
        }

        [Fact]
        public async Task ToSingleAsync_Should_CreateInstance_When_RootIsNull()
        {
            var multipleResultMap = new MultipleResultMap
            {
                Root = null,
                Results = new List<ResultEntry>()
            };

            var mockReader = new Mock<DbDataReader>();

            var context = CreateContext<TestEntity>(mockReader, multipleResultMap);

            var result = await _sut.ToSingleAsync<TestEntity>(context);

            result.Should().NotBeNull();
            result.Should().BeOfType<TestEntity>();
        }

        [Fact]
        public void ToList_Should_Throw_When_Called()
        {
            var context = CreateContext<TestEntity>(new Mock<DbDataReader>());

            var act = () => _sut.ToList<TestEntity>(context);

            act.Should().Throw<SmartSqlException>()
                .WithMessage("MultipleResultDeserializer can not support ToList.");
        }

        [Fact]
        public async Task ToListAsync_Should_Throw_When_Called()
        {
            var context = CreateContext<TestEntity>(new Mock<DbDataReader>());

            var act = () => _sut.ToListAsync<TestEntity>(context);

            await act.Should().ThrowAsync<SmartSqlException>()
                .WithMessage("MultipleResultDeserializer can not support ToListAsync.");
        }

        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    #endregion
}
