using System;
using System.Data.Common;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Middlewares;
using Xunit;

namespace SmartSql.Test.Unit.Middlewares;

public class TransactionMiddlewareTests
{
    private static ExecutionContext CreateContext(
        IsolationLevel? transaction = null,
        DbTransaction existingTransaction = null)
    {
        var mockSession = new Mock<IDbSession>();
        mockSession.Setup(s => s.Transaction).Returns(existingTransaction);

        var request = new RequestContext<object>();
        request.Transaction = transaction;

        return new ExecutionContext
        {
            Request = request,
            Result = new SingleResultContext<int>(),
            SmartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { ParameterPrefix = "@" }
                }
            },
            DbSession = mockSession.Object
        };
    }

    private static (TransactionMiddleware middleware, Mock<IDbSession> mockSession) CreateMiddleware(
        IsolationLevel? transaction = null,
        DbTransaction existingTransaction = null)
    {
        var mockSession = new Mock<IDbSession>();
        mockSession.Setup(s => s.Transaction).Returns(existingTransaction);
        mockSession.Setup(s => s.BeginTransaction(It.IsAny<IsolationLevel>()));
        mockSession.Setup(s => s.CommitTransaction());
        mockSession.Setup(s => s.RollbackTransaction());

        var request = new RequestContext<object>();
        request.Transaction = transaction;

        var context = new ExecutionContext
        {
            Request = request,
            Result = new SingleResultContext<int>(),
            SmartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { ParameterPrefix = "@" }
                }
            },
            DbSession = mockSession.Object
        };

        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, context.SmartSqlConfig);

        var middleware = new TransactionMiddleware();
        middleware.SetupSmartSql(builder);

        return (middleware, mockSession);
    }

    [Fact]
    public void Should_HaveOrder300()
    {
        var middleware = new TransactionMiddleware();

        middleware.Order.Should().Be(300);
    }

    [Fact]
    public void Should_InvokeNext_When_TransactionAlreadyExists()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var mockTransaction = new Mock<DbTransaction>().Object;
        mockSession.Setup(s => s.Transaction).Returns(mockTransaction);

        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<int>(It.IsAny<ExecutionContext>()))
            .Callback<ExecutionContext>(ctx => nextCalled = true);
        middleware.Next = mockNext.Object;

        var context = CreateContext(transaction: null, existingTransaction: mockTransaction);
        context.DbSession = mockSession.Object;

        middleware.Invoke<int>(context);

        nextCalled.Should().BeTrue();
        mockSession.Verify(s => s.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Never);
    }

    [Fact]
    public void Should_InvokeNext_When_NoTransactionRequested()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<int>(It.IsAny<ExecutionContext>()))
            .Callback<ExecutionContext>(ctx => nextCalled = true);
        middleware.Next = mockNext.Object;

        var context = CreateContext(transaction: null);
        context.DbSession = mockSession.Object;

        middleware.Invoke<int>(context);

        nextCalled.Should().BeTrue();
        mockSession.Verify(s => s.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Never);
    }

    [Fact]
    public void Should_WrapInTransaction_When_TransactionIsRequested()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var context = CreateContext(transaction: IsolationLevel.ReadCommitted);
        context.DbSession = mockSession.Object;

        middleware.Invoke<int>(context);

        mockSession.Verify(s => s.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        mockSession.Verify(s => s.CommitTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeNextAsync_When_TransactionAlreadyExists()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var mockTransaction = new Mock<DbTransaction>().Object;
        mockSession.Setup(s => s.Transaction).Returns(mockTransaction);

        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<int>(It.IsAny<ExecutionContext>()))
            .Callback(() => nextCalled = true)
            .Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        var context = CreateContext(transaction: null, existingTransaction: mockTransaction);
        context.DbSession = mockSession.Object;

        await middleware.InvokeAsync<int>(context);

        nextCalled.Should().BeTrue();
        mockSession.Verify(s => s.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Never);
    }

    [Fact]
    public async Task Should_InvokeNextAsync_When_NoTransactionRequested()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<int>(It.IsAny<ExecutionContext>()))
            .Callback(() => nextCalled = true)
            .Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        var context = CreateContext(transaction: null);
        context.DbSession = mockSession.Object;

        await middleware.InvokeAsync<int>(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_WrapInTransactionAsync_When_TransactionIsRequested()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var context = CreateContext(transaction: IsolationLevel.Serializable);
        context.DbSession = mockSession.Object;

        await middleware.InvokeAsync<int>(context);

        mockSession.Verify(s => s.BeginTransaction(IsolationLevel.Serializable), Times.Once);
        mockSession.Verify(s => s.CommitTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_RollbackAsync_When_TransactionHandlerThrows()
    {
        var (middleware, mockSession) = CreateMiddleware();
        var context = CreateContext(transaction: IsolationLevel.ReadCommitted);
        context.DbSession = mockSession.Object;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<int>(It.IsAny<ExecutionContext>()))
            .ThrowsAsync(new InvalidOperationException("handler error"));
        middleware.Next = mockNext.Object;

        var act = async () => await middleware.InvokeAsync<int>(context);

        await act.Should().ThrowAsync<InvalidOperationException>();
        mockSession.Verify(s => s.RollbackTransaction(), Times.Once);
    }
}
