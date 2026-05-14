using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit.DbSession;

public class InvokeSucceedListenerTests
{
    private readonly InvokeSucceedListener _listener = new();

    private static Mock<IDbSession> CreateMockDbSession(Guid? id = null)
    {
        var mockSession = new Mock<IDbSession>();
        mockSession.SetupGet(s => s.Id).Returns(id ?? Guid.NewGuid());
        return mockSession;
    }

    private static ExecutionContext CreateExecutionContext(IDbSession dbSession)
    {
        return new ExecutionContext
        {
            DbSession = dbSession,
            SmartSqlConfig = new SmartSqlConfig()
        };
    }

    #region BindDbSessionEvent

    [Fact]
    public void Should_RaiseInvokeSucceeded_When_InvokedWithoutTransaction()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        mockSession.SetupGet(s => s.Transaction).Returns((System.Data.Common.DbTransaction)null);
        _listener.BindDbSessionEvent(mockSession.Object);

        InvokeSucceededEventArgs capturedArgs = null;
        _listener.InvokeSucceeded += (_, args) => capturedArgs = args;

        var executionContext = CreateExecutionContext(mockSession.Object);

        // Act
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = executionContext });

        // Assert
        capturedArgs.Should().NotBeNull();
        capturedArgs.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_NotRaiseInvokeSucceeded_When_InvokedWithinTransaction()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        var mockTransaction = new Mock<System.Data.Common.DbTransaction>();
        mockSession.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);
        _listener.BindDbSessionEvent(mockSession.Object);

        InvokeSucceededEventArgs capturedArgs = null;
        _listener.InvokeSucceeded += (_, args) => capturedArgs = args;

        var executionContext = CreateExecutionContext(mockSession.Object);

        // Act
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = executionContext });

        // Assert
        capturedArgs.Should().BeNull();
    }

    [Fact]
    public void Should_RaiseQueuedEvents_When_TransactionCommitted()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        var mockTransaction = new Mock<System.Data.Common.DbTransaction>();
        mockSession.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);
        _listener.BindDbSessionEvent(mockSession.Object);

        var executionContext = CreateExecutionContext(mockSession.Object);
        var capturedContexts = new List<ExecutionContext>();
        _listener.InvokeSucceeded += (_, args) => capturedContexts.Add(args.ExecutionContext);

        // Act - invoke within transaction (should queue)
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = executionContext });

        // Assert - not yet raised
        capturedContexts.Should().BeEmpty();

        // Act - commit transaction
        mockSession.SetupGet(s => s.Transaction).Returns((System.Data.Common.DbTransaction)null);
        mockSession.Raise(s => s.Committed += null, mockSession.Object, DbSessionEventArgs.None);

        // Assert - now raised
        capturedContexts.Should().HaveCount(1);
        capturedContexts[0].Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_NotRaiseQueuedEvents_When_TransactionRollbacked()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        var mockTransaction = new Mock<System.Data.Common.DbTransaction>();
        mockSession.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);
        _listener.BindDbSessionEvent(mockSession.Object);

        var executionContext = CreateExecutionContext(mockSession.Object);
        var capturedContexts = new List<ExecutionContext>();
        _listener.InvokeSucceeded += (_, args) => capturedContexts.Add(args.ExecutionContext);

        // Act - invoke within transaction (should queue)
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = executionContext });

        // Act - rollback transaction
        mockSession.Raise(s => s.Rollbacked += null, mockSession.Object, DbSessionEventArgs.None);

        // Assert - queued events should be discarded
        capturedContexts.Should().BeEmpty();
    }

    [Fact]
    public void Should_RaiseMultipleQueuedEvents_When_TransactionCommitted()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        var mockTransaction = new Mock<System.Data.Common.DbTransaction>();
        mockSession.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);
        _listener.BindDbSessionEvent(mockSession.Object);

        var ec1 = CreateExecutionContext(mockSession.Object);
        var ec2 = CreateExecutionContext(mockSession.Object);
        var capturedContexts = new List<ExecutionContext>();
        _listener.InvokeSucceeded += (_, args) => capturedContexts.Add(args.ExecutionContext);

        // Act - invoke twice within transaction
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = ec1 });
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = ec2 });

        // Act - commit transaction
        mockSession.SetupGet(s => s.Transaction).Returns((System.Data.Common.DbTransaction)null);
        mockSession.Raise(s => s.Committed += null, mockSession.Object, DbSessionEventArgs.None);

        // Assert
        capturedContexts.Should().HaveCount(2);
        capturedContexts[0].Should().BeSameAs(ec1);
        capturedContexts[1].Should().BeSameAs(ec2);
    }

    [Fact]
    public void Should_DiscardQueuedEvents_When_SessionDisposed()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        var mockTransaction = new Mock<System.Data.Common.DbTransaction>();
        mockSession.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);
        _listener.BindDbSessionEvent(mockSession.Object);

        var executionContext = CreateExecutionContext(mockSession.Object);
        var capturedContexts = new List<ExecutionContext>();
        _listener.InvokeSucceeded += (_, args) => capturedContexts.Add(args.ExecutionContext);

        // Act - invoke within transaction
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = executionContext });

        // Act - dispose session
        mockSession.Raise(s => s.Disposed += null, mockSession.Object, DbSessionEventArgs.None);

        // Assert
        capturedContexts.Should().BeEmpty();
    }

    [Fact]
    public void Should_HandleMultipleSessions_When_BothHaveTransactions()
    {
        // Arrange
        var mockSession1 = CreateMockDbSession();
        var mockSession2 = CreateMockDbSession();
        var mockTransaction = new Mock<System.Data.Common.DbTransaction>();

        mockSession1.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);
        mockSession2.SetupGet(s => s.Transaction).Returns(mockTransaction.Object);

        _listener.BindDbSessionEvent(mockSession1.Object);
        _listener.BindDbSessionEvent(mockSession2.Object);

        var ec1 = CreateExecutionContext(mockSession1.Object);
        var ec2 = CreateExecutionContext(mockSession2.Object);
        var capturedContexts = new List<ExecutionContext>();
        _listener.InvokeSucceeded += (_, args) => capturedContexts.Add(args.ExecutionContext);

        // Act
        mockSession1.Raise(s => s.Invoked += null,
            mockSession1.Object, new DbSessionInvokedEventArgs { ExecutionContext = ec1 });
        mockSession2.Raise(s => s.Invoked += null,
            mockSession2.Object, new DbSessionInvokedEventArgs { ExecutionContext = ec2 });

        // Commit session1 only
        mockSession1.SetupGet(s => s.Transaction).Returns((System.Data.Common.DbTransaction)null);
        mockSession1.Raise(s => s.Committed += null, mockSession1.Object, DbSessionEventArgs.None);

        // Assert - only session1's context raised
        capturedContexts.Should().HaveCount(1);
        capturedContexts[0].Should().BeSameAs(ec1);
    }

    [Fact]
    public void Should_RaiseImmediately_When_InvokeOutsideTransaction()
    {
        // Arrange
        var mockSession = CreateMockDbSession();
        mockSession.SetupGet(s => s.Transaction).Returns((System.Data.Common.DbTransaction)null);
        _listener.BindDbSessionEvent(mockSession.Object);

        var ec1 = CreateExecutionContext(mockSession.Object);
        var ec2 = CreateExecutionContext(mockSession.Object);
        var capturedContexts = new List<ExecutionContext>();
        _listener.InvokeSucceeded += (_, args) => capturedContexts.Add(args.ExecutionContext);

        // Act - invoke twice outside transaction
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = ec1 });
        mockSession.Raise(s => s.Invoked += null,
            mockSession.Object, new DbSessionInvokedEventArgs { ExecutionContext = ec2 });

        // Assert - both raised immediately
        capturedContexts.Should().HaveCount(2);
    }

    #endregion
}
