using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Deserializer;
using SmartSql.Middlewares;
using Xunit;

namespace SmartSql.Test.Unit.Middlewares;

public class ResultHandlerMiddlewareTests
{
    private static (ResultHandlerMiddleware middleware, Mock<IDeserializerFactory> mockFactory, SmartSqlConfig config)
        CreateMiddleware()
    {
        var mockFactory = new Mock<IDeserializerFactory>();
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = new DbProvider { ParameterPrefix = "@" }
            }
        };

        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        var middleware = new ResultHandlerMiddleware();
        middleware.SetupSmartSql(builder);

        var factoryField = typeof(ResultHandlerMiddleware).GetField("_deserializerFactory",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        factoryField.SetValue(middleware, mockFactory.Object);

        return (middleware, mockFactory, config);
    }

    private static ExecutionContext CreateContext<T>(bool isList = false, DataReaderWrapper dataReader = null)
    {
        var request = new RequestContext<object>();

        ResultContext result = isList
            ? new ListResultContext<T>()
            : new SingleResultContext<T>();

        return new ExecutionContext
        {
            Request = request,
            Result = result,
            SmartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { ParameterPrefix = "@" }
                }
            },
            DbSession = new Mock<IDbSession>().Object,
            DataReaderWrapper = dataReader
        };
    }

    [Fact]
    public void Should_HaveOrder600()
    {
        var middleware = new ResultHandlerMiddleware();

        middleware.Order.Should().Be(600);
    }

    [Fact]
    public void Should_SetSingleResult_When_InvokeWithSingleResultContext()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingle<int>(It.IsAny<ExecutionContext>())).Returns(42);

        var mockReader = new Mock<DbDataReader>();
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<int>(isList: false, dataReader: wrapper);

        middleware.Invoke<int>(context);

        context.Result.GetData().Should().Be(42);
        context.Result.End.Should().BeTrue();
    }

    [Fact]
    public void Should_SetListResult_When_InvokeWithListResultContext()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        var expectedList = new List<int> { 1, 2, 3 };
        mockDeserializer.Setup(d => d.ToList<int>(It.IsAny<ExecutionContext>())).Returns(expectedList);

        var mockReader = new Mock<DbDataReader>();
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<int>(isList: true, dataReader: wrapper);

        middleware.Invoke<int>(context);

        context.Result.GetData().Should().BeEquivalentTo(new[] { 1, 2, 3 });
        context.Result.End.Should().BeTrue();
    }

    [Fact]
    public void Should_CloseDataReader_When_InvokeCompletes()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingle<int>(It.IsAny<ExecutionContext>())).Returns(1);

        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.Close());
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<int>(isList: false, dataReader: wrapper);

        middleware.Invoke<int>(context);

        mockReader.Verify(r => r.Close(), Times.AtLeastOnce());
    }

    [Fact]
    public void Should_CloseDataReader_When_DeserializerThrows()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingle<int>(It.IsAny<ExecutionContext>()))
            .Throws(new InvalidOperationException("deserialization error"));

        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.Close());
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<int>(isList: false, dataReader: wrapper);

        var act = () => middleware.Invoke<int>(context);

        act.Should().Throw<InvalidOperationException>();
        mockReader.Verify(r => r.Close(), Times.AtLeastOnce());
    }

    [Fact]
    public async Task Should_SetSingleResultAsync_When_InvokeAsyncWithSingleResultContext()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(string), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingleAsync<string>(It.IsAny<ExecutionContext>()))
            .ReturnsAsync("test-result");

        var mockReader = new Mock<DbDataReader>();
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<string>(isList: false, dataReader: wrapper);

        await middleware.InvokeAsync<string>(context);

        context.Result.GetData().Should().Be("test-result");
        context.Result.End.Should().BeTrue();
    }

    [Fact]
    public async Task Should_SetListResultAsync_When_InvokeAsyncWithListResultContext()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(string), false))
            .Returns(mockDeserializer.Object);
        var expectedList = new List<string> { "a", "b" };
        mockDeserializer.Setup(d => d.ToListAsync<string>(It.IsAny<ExecutionContext>()))
            .ReturnsAsync(expectedList);

        var mockReader = new Mock<DbDataReader>();
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<string>(isList: true, dataReader: wrapper);

        await middleware.InvokeAsync<string>(context);

        context.Result.GetData().Should().BeEquivalentTo(new[] { "a", "b" });
        context.Result.End.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CloseDataReaderAsync_When_InvokeAsyncCompletes()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingleAsync<int>(It.IsAny<ExecutionContext>()))
            .ReturnsAsync(99);

        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.Close());
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<int>(isList: false, dataReader: wrapper);

        await middleware.InvokeAsync<int>(context);

        mockReader.Verify(r => r.Close(), Times.AtLeastOnce());
    }

    [Fact]
    public async Task Should_CloseDataReaderAsync_When_DeserializerThrowsAsync()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingleAsync<int>(It.IsAny<ExecutionContext>()))
            .ThrowsAsync(new InvalidOperationException("async error"));

        var mockReader = new Mock<DbDataReader>();
        mockReader.Setup(r => r.Close());
        var wrapper = new DataReaderWrapper(mockReader.Object);
        var context = CreateContext<int>(isList: false, dataReader: wrapper);

        var act = async () => await middleware.InvokeAsync<int>(context);

        await act.Should().ThrowAsync<InvalidOperationException>();
        mockReader.Verify(r => r.Close(), Times.AtLeastOnce());
    }

    [Fact]
    public void Should_HandleNullDataReader_When_InvokeWithoutDataReader()
    {
        var (middleware, mockFactory, _) = CreateMiddleware();
        var mockDeserializer = new Mock<IDataReaderDeserializer>();
        mockFactory.Setup(f => f.Get(It.IsAny<ExecutionContext>(), typeof(int), false))
            .Returns(mockDeserializer.Object);
        mockDeserializer.Setup(d => d.ToSingle<int>(It.IsAny<ExecutionContext>())).Returns(1);

        var context = CreateContext<int>(isList: false, dataReader: null);

        var act = () => middleware.Invoke<int>(context);

        act.Should().NotThrow();
    }
}
