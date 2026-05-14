using System;
using FluentAssertions;
using Moq;
using SmartSql.DbSession;
using SmartSql.Diagnostics;
using Xunit;

namespace SmartSql.Test.Unit.Diagnostics;

public class EventDataTests
{
    private static readonly Guid TestOperationId = Guid.NewGuid();
    private const string TestOperation = "TestMethod";

    #region EventData

    [Fact]
    public void Should_SetProperties_When_EventDataConstructed()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        const string operation = "TestOp";

        // Act
        var eventData = new EventData(operationId, operation);

        // Assert
        eventData.OperationId.Should().Be(operationId);
        eventData.Operation.Should().Be(operation);
    }

    [Fact]
    public void Should_SetProperties_When_EventDataConstructedWithEmptyGuid()
    {
        // Act
        var eventData = new EventData(Guid.Empty, "");

        // Assert
        eventData.OperationId.Should().Be(Guid.Empty);
        eventData.Operation.Should().BeEmpty();
    }

    #endregion

    #region IErrorEventData

    [Fact]
    public void Should_ImplementIErrorEventData_When_CommandExecuterExecuteErrorEventDataCreated()
    {
        // Act
        var errorData = new CommandExecuterExecuteErrorEventData(TestOperationId, TestOperation);

        // Assert
        errorData.Should().BeAssignableTo<IErrorEventData>();
    }

    #endregion

    #region DbSessionEventData

    [Fact]
    public void Should_SetProperties_When_DbSessionEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.OperationId.Should().Be(TestOperationId);
        eventData.Operation.Should().Be(TestOperation);
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetDbSessionToNull_When_NotAssigned()
    {
        // Act
        var eventData = new DbSessionEventData(TestOperationId, TestOperation);

        // Assert
        eventData.DbSession.Should().BeNull();
    }

    #endregion

    #region DbSessionInvokeEventData

    [Fact]
    public void Should_SetProperties_When_DbSessionInvokeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var executionContext = new ExecutionContext
        {
            DbSession = mockSession
        };

        // Act
        var eventData = new DbSessionInvokeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            ExecutionContext = executionContext
        };

        // Assert
        eventData.OperationId.Should().Be(TestOperationId);
        eventData.Operation.Should().Be(TestOperation);
        eventData.DbSession.Should().BeSameAs(mockSession);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_InheritDbSessionEventData_When_DbSessionInvokeEventDataCreated()
    {
        // Act
        var eventData = new DbSessionInvokeEventData(TestOperationId, TestOperation);

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
    }

    #endregion

    #region CommandExecuterEventData

    [Fact]
    public void Should_SetProperties_When_CommandExecuterEventDataConstructed()
    {
        // Arrange
        var executionContext = new ExecutionContext();

        // Act
        var eventData = new CommandExecuterEventData(TestOperationId, TestOperation)
        {
            ExecutionContext = executionContext
        };

        // Assert
        eventData.OperationId.Should().Be(TestOperationId);
        eventData.Operation.Should().Be(TestOperation);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_SetExecutionContextToNull_When_NotAssigned()
    {
        // Act
        var eventData = new CommandExecuterEventData(TestOperationId, TestOperation);

        // Assert
        eventData.ExecutionContext.Should().BeNull();
    }

    #endregion

    #region DbSession Open EventData

    [Fact]
    public void Should_SetProperties_When_DbSessionOpenBeforeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionOpenBeforeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionOpenAfterEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionOpenAfterEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionOpenErrorEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var exception = new InvalidOperationException("Open failed");

        // Act
        var eventData = new DbSessionOpenErrorEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            Exception = exception
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region DbSession BeginTransaction EventData

    [Fact]
    public void Should_SetProperties_When_DbSessionBeginTransactionBeforeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionBeginTransactionBeforeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionBeginTransactionAfterEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionBeginTransactionAfterEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionBeginTransactionErrorEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var exception = new InvalidOperationException("BeginTransaction failed");

        // Act
        var eventData = new DbSessionBeginTransactionErrorEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            Exception = exception
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region DbSession Commit EventData

    [Fact]
    public void Should_SetProperties_When_DbSessionCommitBeforeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionCommitBeforeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionCommitAfterEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionCommitAfterEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionCommitErrorEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var exception = new InvalidOperationException("Commit failed");

        // Act
        var eventData = new DbSessionCommitErrorEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            Exception = exception
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region DbSession Rollback EventData

    [Fact]
    public void Should_SetProperties_When_DbSessionRollbackBeforeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionRollbackBeforeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionRollbackAfterEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionRollbackAfterEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionRollbackErrorEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var exception = new InvalidOperationException("Rollback failed");

        // Act
        var eventData = new DbSessionRollbackErrorEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            Exception = exception
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region DbSession Dispose EventData

    [Fact]
    public void Should_SetProperties_When_DbSessionDisposeBeforeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionDisposeBeforeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionDisposeAfterEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;

        // Act
        var eventData = new DbSessionDisposeAfterEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionDisposeErrorEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var exception = new InvalidOperationException("Dispose failed");

        // Act
        var eventData = new DbSessionDisposeErrorEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            Exception = exception
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region DbSession Invoke EventData

    [Fact]
    public void Should_SetProperties_When_DbSessionInvokeBeforeEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var executionContext = new ExecutionContext { DbSession = mockSession };

        // Act
        var eventData = new DbSessionInvokeBeforeEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            ExecutionContext = executionContext
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionInvokeEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionInvokeAfterEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var executionContext = new ExecutionContext { DbSession = mockSession };

        // Act
        var eventData = new DbSessionInvokeAfterEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            ExecutionContext = executionContext
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionInvokeEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_SetProperties_When_DbSessionInvokeErrorEventDataConstructed()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>().Object;
        var executionContext = new ExecutionContext { DbSession = mockSession };
        var exception = new InvalidOperationException("Invoke failed");

        // Act
        var eventData = new DbSessionInvokeErrorEventData(TestOperationId, TestOperation)
        {
            DbSession = mockSession,
            Exception = exception,
            ExecutionContext = executionContext
        };

        // Assert
        eventData.Should().BeAssignableTo<DbSessionInvokeEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.DbSession.Should().BeSameAs(mockSession);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region CommandExecuter Execute EventData

    [Fact]
    public void Should_SetProperties_When_CommandExecuterExecuteBeforeEventDataConstructed()
    {
        // Arrange
        var executionContext = new ExecutionContext();

        // Act
        var eventData = new CommandExecuterExecuteBeforeEventData(TestOperationId, TestOperation)
        {
            ExecutionContext = executionContext
        };

        // Assert
        eventData.Should().BeAssignableTo<CommandExecuterEventData>();
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_SetProperties_When_CommandExecuterExecuteAfterEventDataConstructed()
    {
        // Arrange
        var executionContext = new ExecutionContext();

        // Act
        var eventData = new CommandExecuterExecuteAfterEventData(TestOperationId, TestOperation)
        {
            ExecutionContext = executionContext
        };

        // Assert
        eventData.Should().BeAssignableTo<CommandExecuterEventData>();
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_SetProperties_When_CommandExecuterExecuteErrorEventDataConstructed()
    {
        // Arrange
        var executionContext = new ExecutionContext();
        var exception = new InvalidOperationException("Execute failed");

        // Act
        var eventData = new CommandExecuterExecuteErrorEventData(TestOperationId, TestOperation)
        {
            ExecutionContext = executionContext,
            Exception = exception
        };

        // Assert
        eventData.Should().BeAssignableTo<CommandExecuterEventData>();
        eventData.Should().BeAssignableTo<IErrorEventData>();
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion
}
