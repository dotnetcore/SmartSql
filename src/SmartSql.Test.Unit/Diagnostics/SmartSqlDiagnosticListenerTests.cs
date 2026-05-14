using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Moq;
using SmartSql.DbSession;
using SmartSql.Diagnostics;
using Xunit;

namespace SmartSql.Test.Unit.Diagnostics;

public class SmartSqlDiagnosticListenerTests : IDisposable
{
    private readonly DiagnosticListener _listener;
    private readonly List<(string Name, object Value)> _recordedEvents;
    private readonly IDisposable _subscription;
    private readonly Mock<IDbSession> _mockSession;

    public SmartSqlDiagnosticListenerTests()
    {
        _listener = new DiagnosticListener(SmartSqlDiagnosticListenerExtensions.SMART_SQL_DIAGNOSTIC_LISTENER);
        _recordedEvents = new List<(string Name, object Value)>();
        _subscription = _listener.Subscribe(new DiagnosticObserver(_recordedEvents));
        _mockSession = new Mock<IDbSession>();
    }

    public void Dispose()
    {
        _subscription.Dispose();
        _listener.Dispose();
    }

    private ExecutionContext CreateExecutionContext()
    {
        return new ExecutionContext
        {
            DbSession = _mockSession.Object
        };
    }

    #region Constants

    [Fact]
    public void Should_HaveCorrectListenerName_When_AccessingConstant()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_DIAGNOSTIC_LISTENER
            .Should().Be("SmartSqlDiagnosticListener");
    }

    [Fact]
    public void Should_HaveCorrectPrefix_When_AccessingConstant()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_PREFIX
            .Should().Be("SmartSql.");
    }

    [Fact]
    public void Should_HaveNonEmptyGuid_When_InstanceCreated()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.Instance.Should().NotBeNull();
        SmartSqlDiagnosticListenerExtensions.Instance.Name
            .Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_DIAGNOSTIC_LISTENER);
    }

    #endregion

    #region Session Open

    [Fact]
    public void Should_WriteBeforeEvent_When_DbSessionOpenBeforeCalled()
    {
        // Act
        var operationId = _listener.WriteDbSessionOpenBefore(_mockSession.Object);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_OPEN);
        var eventData = _recordedEvents[0].Value.As<DbSessionOpenBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_DbSessionOpenAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act
        _listener.WriteDbSessionOpenAfter(operationId, _mockSession.Object);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_OPEN);
        var eventData = _recordedEvents[0].Value.As<DbSessionOpenAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_DbSessionOpenErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var exception = new InvalidOperationException("Open failed");

        // Act
        _listener.WriteDbSessionOpenError(operationId, _mockSession.Object, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_OPEN);
        var eventData = _recordedEvents[0].Value.As<DbSessionOpenErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region Session BeginTransaction

    [Fact]
    public void Should_WriteBeforeEvent_When_DbSessionBeginTransactionBeforeCalled()
    {
        // Act
        var operationId = _listener.WriteDbSessionBeginTransactionBefore(_mockSession.Object);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_BEGINTRANSACTION);
        var eventData = _recordedEvents[0].Value.As<DbSessionBeginTransactionBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_DbSessionBeginTransactionAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act
        _listener.WriteDbSessionBeginTransactionAfter(operationId, _mockSession.Object);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_BEGINTRANSACTION);
        var eventData = _recordedEvents[0].Value.As<DbSessionBeginTransactionAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_DbSessionBeginTransactionErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var exception = new InvalidOperationException("BeginTransaction failed");

        // Act
        _listener.WriteDbSessionBeginTransactionError(operationId, _mockSession.Object, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_BEGINTRANSACTION);
        var eventData = _recordedEvents[0].Value.As<DbSessionBeginTransactionErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region Session Commit

    [Fact]
    public void Should_WriteBeforeEvent_When_DbSessionCommitBeforeCalled()
    {
        // Act
        var operationId = _listener.WriteDbSessionCommitBefore(_mockSession.Object);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_COMMIT);
        var eventData = _recordedEvents[0].Value.As<DbSessionCommitBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_DbSessionCommitAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act
        _listener.WriteDbSessionCommitAfter(operationId, _mockSession.Object);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_COMMIT);
        var eventData = _recordedEvents[0].Value.As<DbSessionCommitAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_DbSessionCommitErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var exception = new InvalidOperationException("Commit failed");

        // Act
        _listener.WriteDbSessionCommitError(operationId, _mockSession.Object, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_COMMIT);
        var eventData = _recordedEvents[0].Value.As<DbSessionCommitErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region Session Rollback

    [Fact]
    public void Should_WriteBeforeEvent_When_DbSessionRollbackBeforeCalled()
    {
        // Act
        var operationId = _listener.WriteDbSessionRollbackBefore(_mockSession.Object);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_ROLLBACK);
        var eventData = _recordedEvents[0].Value.As<DbSessionRollbackBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_DbSessionRollbackAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act
        _listener.WriteDbSessionRollbackAfter(operationId, _mockSession.Object);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_ROLLBACK);
        var eventData = _recordedEvents[0].Value.As<DbSessionRollbackAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_DbSessionRollbackErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var exception = new InvalidOperationException("Rollback failed");

        // Act
        _listener.WriteDbSessionRollbackError(operationId, _mockSession.Object, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_ROLLBACK);
        var eventData = _recordedEvents[0].Value.As<DbSessionRollbackErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region Session Dispose

    [Fact]
    public void Should_WriteBeforeEvent_When_DbSessionDisposeBeforeCalled()
    {
        // Act
        var operationId = _listener.WriteDbSessionDisposeBefore(_mockSession.Object);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_DISPOSE);
        var eventData = _recordedEvents[0].Value.As<DbSessionDisposeBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_DbSessionDisposeAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();

        // Act
        _listener.WriteDbSessionDisposeAfter(operationId, _mockSession.Object);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_DISPOSE);
        var eventData = _recordedEvents[0].Value.As<DbSessionDisposeAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_DbSessionDisposeErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var exception = new InvalidOperationException("Dispose failed");

        // Act
        _listener.WriteDbSessionDisposeError(operationId, _mockSession.Object, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_DISPOSE);
        var eventData = _recordedEvents[0].Value.As<DbSessionDisposeErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region Session Invoke

    [Fact]
    public void Should_WriteBeforeEvent_When_DbSessionInvokeBeforeCalled()
    {
        // Arrange
        var executionContext = CreateExecutionContext();

        // Act
        var operationId = _listener.WriteDbSessionInvokeBefore(executionContext);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_INVOKE);
        var eventData = _recordedEvents[0].Value.As<DbSessionInvokeBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_DbSessionInvokeAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var executionContext = CreateExecutionContext();

        // Act
        _listener.WriteDbSessionInvokeAfter(operationId, executionContext);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_INVOKE);
        var eventData = _recordedEvents[0].Value.As<DbSessionInvokeAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_DbSessionInvokeErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var executionContext = CreateExecutionContext();
        var exception = new InvalidOperationException("Invoke failed");

        // Act
        _listener.WriteDbSessionInvokeError(operationId, executionContext, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_INVOKE);
        var eventData = _recordedEvents[0].Value.As<DbSessionInvokeErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.DbSession.Should().BeSameAs(_mockSession.Object);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region CommandExecuter Execute

    [Fact]
    public void Should_WriteBeforeEvent_When_CommandExecuterExecuteBeforeCalled()
    {
        // Arrange
        var executionContext = CreateExecutionContext();

        // Act
        var operationId = _listener.WriteCommandExecuterExecuteBefore(executionContext);

        // Assert
        operationId.Should().NotBe(Guid.Empty);
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_COMMAND_EXECUTER_EXECUTE);
        var eventData = _recordedEvents[0].Value.As<CommandExecuterExecuteBeforeEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_WriteAfterEvent_When_CommandExecuterExecuteAfterCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var executionContext = CreateExecutionContext();

        // Act
        _listener.WriteCommandExecuterExecuteAfter(operationId, executionContext);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_COMMAND_EXECUTER_EXECUTE);
        var eventData = _recordedEvents[0].Value.As<CommandExecuterExecuteAfterEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
    }

    [Fact]
    public void Should_WriteErrorEvent_When_CommandExecuterExecuteErrorCalled()
    {
        // Arrange
        var operationId = Guid.NewGuid();
        var executionContext = CreateExecutionContext();
        var exception = new InvalidOperationException("Execute failed");

        // Act
        _listener.WriteCommandExecuterExecuteError(operationId, executionContext, exception);

        // Assert
        _recordedEvents.Should().ContainSingle();
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_COMMAND_EXECUTER_EXECUTE);
        var eventData = _recordedEvents[0].Value.As<CommandExecuterExecuteErrorEventData>();
        eventData.OperationId.Should().Be(operationId);
        eventData.ExecutionContext.Should().BeSameAs(executionContext);
        eventData.Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region IsEnabled Guard

    [Fact]
    public void Should_ReturnEmptyGuid_When_BeforeEventIsNotEnabled()
    {
        // Arrange - use a listener that disables all events
        var disabledListener = new DiagnosticListener("DisabledListener_" + Guid.NewGuid());
        using var subscription = disabledListener.Subscribe(
            new DiagnosticObserver(_recordedEvents),
            name => false);

        // Act
        var operationId = disabledListener.WriteDbSessionOpenBefore(_mockSession.Object);

        // Assert
        operationId.Should().Be(Guid.Empty);
        _recordedEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotWriteAfterEvent_When_EventIsNotEnabled()
    {
        // Arrange - use a listener that disables after events
        var selectiveListener = new DiagnosticListener("SelectiveListener_" + Guid.NewGuid());
        var selectiveEvents = new List<(string Name, object Value)>();
        using var subscription = selectiveListener.Subscribe(
            new DiagnosticObserver(selectiveEvents),
            name => name.Contains("Before"));

        // Act
        selectiveListener.WriteDbSessionOpenAfter(Guid.NewGuid(), _mockSession.Object);

        // Assert
        selectiveEvents.Should().BeEmpty();
    }

    [Fact]
    public void Should_NotWriteErrorEvent_When_EventIsNotEnabled()
    {
        // Arrange
        var selectiveListener = new DiagnosticListener("SelectiveListener_" + Guid.NewGuid());
        var selectiveEvents = new List<(string Name, object Value)>();
        using var subscription = selectiveListener.Subscribe(
            new DiagnosticObserver(selectiveEvents),
            name => !name.Contains("Error"));

        // Act
        selectiveListener.WriteDbSessionOpenError(Guid.NewGuid(), _mockSession.Object,
            new InvalidOperationException("test"));

        // Assert
        selectiveEvents.Should().BeEmpty();
    }

    #endregion

    #region Event Name Constants

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingSessionOpenConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_OPEN
            .Should().Be("SmartSql.WriteDbSessionOpenBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_OPEN
            .Should().Be("SmartSql.WriteDbSessionOpenAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_OPEN
            .Should().Be("SmartSql.WriteDbSessionOpenError");
    }

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingSessionBeginTransactionConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_BEGINTRANSACTION
            .Should().Be("SmartSql.WriteDbSessionBeginTransactionBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_BEGINTRANSACTION
            .Should().Be("SmartSql.WriteDbSessionBeginTransactionAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_BEGINTRANSACTION
            .Should().Be("SmartSql.WriteDbSessionBeginTransactionError");
    }

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingSessionCommitConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_COMMIT
            .Should().Be("SmartSql.WriteDbSessionCommitBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_COMMIT
            .Should().Be("SmartSql.WriteDbSessionCommitAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_COMMIT
            .Should().Be("SmartSql.WriteDbSessionCommitError");
    }

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingSessionRollbackConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_ROLLBACK
            .Should().Be("SmartSql.WriteDbSessionRollbackBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_ROLLBACK
            .Should().Be("SmartSql.WriteDbSessionRollbackAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_ROLLBACK
            .Should().Be("SmartSql.WriteDbSessionRollbackError");
    }

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingSessionDisposeConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_DISPOSE
            .Should().Be("SmartSql.WriteDbSessionDisposeBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_DISPOSE
            .Should().Be("SmartSql.WriteDbSessionDisposeAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_DISPOSE
            .Should().Be("SmartSql.WriteDbSessionDisposeError");
    }

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingSessionInvokeConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_INVOKE
            .Should().Be("SmartSql.WriteDbSessionInvokeBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_INVOKE
            .Should().Be("SmartSql.WriteDbSessionInvokeAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_DB_SESSION_INVOKE
            .Should().Be("SmartSql.WriteDbSessionInvokeError");
    }

    [Fact]
    public void Should_HaveCorrectEventNames_When_AccessingCommandExecuterConstants()
    {
        // Assert
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_COMMAND_EXECUTER_EXECUTE
            .Should().Be("SmartSql.WriteCommandExecuterExecuteBefore");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_COMMAND_EXECUTER_EXECUTE
            .Should().Be("SmartSql.WriteCommandExecuterExecuteAfter");
        SmartSqlDiagnosticListenerExtensions.SMART_SQL_ERROR_COMMAND_EXECUTER_EXECUTE
            .Should().Be("SmartSql.WriteCommandExecuterExecuteError");
    }

    #endregion

    #region Full Lifecycle

    [Fact]
    public void Should_WriteAllEvents_When_FullSessionLifecycleExecuted()
    {
        // Arrange
        var executionContext = CreateExecutionContext();

        // Act - simulate full lifecycle
        var openId = _listener.WriteDbSessionOpenBefore(_mockSession.Object);
        _listener.WriteDbSessionOpenAfter(openId, _mockSession.Object);

        var beginId = _listener.WriteDbSessionBeginTransactionBefore(_mockSession.Object);
        _listener.WriteDbSessionBeginTransactionAfter(beginId, _mockSession.Object);

        var invokeId = _listener.WriteDbSessionInvokeBefore(executionContext);
        _listener.WriteDbSessionInvokeAfter(invokeId, executionContext);

        var executeId = _listener.WriteCommandExecuterExecuteBefore(executionContext);
        _listener.WriteCommandExecuterExecuteAfter(executeId, executionContext);

        var commitId = _listener.WriteDbSessionCommitBefore(_mockSession.Object);
        _listener.WriteDbSessionCommitAfter(commitId, _mockSession.Object);

        var disposeId = _listener.WriteDbSessionDisposeBefore(_mockSession.Object);
        _listener.WriteDbSessionDisposeAfter(disposeId, _mockSession.Object);

        // Assert
        _recordedEvents.Should().HaveCount(12);
        _recordedEvents[0].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_OPEN);
        _recordedEvents[1].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_OPEN);
        _recordedEvents[2].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_BEGINTRANSACTION);
        _recordedEvents[3].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_BEGINTRANSACTION);
        _recordedEvents[4].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_INVOKE);
        _recordedEvents[5].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_INVOKE);
        _recordedEvents[6].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_COMMAND_EXECUTER_EXECUTE);
        _recordedEvents[7].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_COMMAND_EXECUTER_EXECUTE);
        _recordedEvents[8].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_COMMIT);
        _recordedEvents[9].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_COMMIT);
        _recordedEvents[10].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_BEFORE_DB_SESSION_DISPOSE);
        _recordedEvents[11].Name.Should().Be(SmartSqlDiagnosticListenerExtensions.SMART_SQL_AFTER_DB_SESSION_DISPOSE);
    }

    [Fact]
    public void Should_WriteAllErrorEvents_When_SessionLifecycleFails()
    {
        // Arrange
        var executionContext = CreateExecutionContext();
        var exception = new InvalidOperationException("Failed");

        // Act - simulate failed lifecycle
        var openId = _listener.WriteDbSessionOpenBefore(_mockSession.Object);
        _listener.WriteDbSessionOpenError(openId, _mockSession.Object, exception);

        var beginId = _listener.WriteDbSessionBeginTransactionBefore(_mockSession.Object);
        _listener.WriteDbSessionBeginTransactionError(beginId, _mockSession.Object, exception);

        var invokeId = _listener.WriteDbSessionInvokeBefore(executionContext);
        _listener.WriteDbSessionInvokeError(invokeId, executionContext, exception);

        var executeId = _listener.WriteCommandExecuterExecuteBefore(executionContext);
        _listener.WriteCommandExecuterExecuteError(executeId, executionContext, exception);

        var commitId = _listener.WriteDbSessionCommitBefore(_mockSession.Object);
        _listener.WriteDbSessionCommitError(commitId, _mockSession.Object, exception);

        var rollbackId = _listener.WriteDbSessionRollbackBefore(_mockSession.Object);
        _listener.WriteDbSessionRollbackError(rollbackId, _mockSession.Object, exception);

        var disposeId = _listener.WriteDbSessionDisposeBefore(_mockSession.Object);
        _listener.WriteDbSessionDisposeError(disposeId, _mockSession.Object, exception);

        // Assert - 7 before + 7 error = 14 events
        _recordedEvents.Should().HaveCount(14);
        _recordedEvents[1].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
        _recordedEvents[3].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
        _recordedEvents[5].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
        _recordedEvents[7].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
        _recordedEvents[9].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
        _recordedEvents[11].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
        _recordedEvents[13].Value.As<IErrorEventData>().Exception.Should().BeSameAs(exception);
    }

    #endregion

    #region Nested DiagnosticObserver

    private class DiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly List<(string Name, object Value)> _events;

        public DiagnosticObserver(List<(string Name, object Value)> events)
        {
            _events = events;
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            _events.Add((value.Key, value.Value));
        }

        public void OnError(Exception error) { }

        public void OnCompleted() { }
    }

    #endregion
}
