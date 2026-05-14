using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit;

public class SmartSqlExtensionsTests
{
    private readonly Mock<IDbSession> _mockSession;
    private readonly Mock<ITransaction> _mockTransaction;

    public SmartSqlExtensionsTests()
    {
        _mockSession = new Mock<IDbSession>();
        _mockTransaction = new Mock<ITransaction>();
    }

    #region IDbSession.TransactionWrap

    [Fact]
    public void Should_CommitTransaction_When_HandlerSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        _mockSession.Object.TransactionWrap(() =>
        {
            invoked = true;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockSession.Verify(s => s.BeginTransaction(), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Once);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public void Should_RollbackTransaction_When_HandlerThrows()
    {
        // Arrange & Act
        Action act = () => _mockSession.Object.TransactionWrap(() =>
        {
            throw new InvalidOperationException("test error");
        });

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("test error");
        _mockSession.Verify(s => s.BeginTransaction(), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Never);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Once);
    }

    [Fact]
    public void Should_CommitTransactionWithIsolationLevel_When_HandlerSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        _mockSession.Object.TransactionWrap(IsolationLevel.Serializable, () =>
        {
            invoked = true;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockSession.Verify(s => s.BeginTransaction(IsolationLevel.Serializable), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Once);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public void Should_RollbackTransactionWithIsolationLevel_When_HandlerThrows()
    {
        // Arrange & Act
        Action act = () => _mockSession.Object.TransactionWrap(IsolationLevel.ReadCommitted, () =>
        {
            throw new InvalidOperationException("test error");
        });

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _mockSession.Verify(s => s.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Never);
    }

    #endregion

    #region IDbSession.TransactionWrapAsync

    [Fact]
    public async Task Should_CommitTransactionAsync_When_AsyncHandlerSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        await _mockSession.Object.TransactionWrapAsync(async () =>
        {
            invoked = true;
            await Task.CompletedTask;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockSession.Verify(s => s.BeginTransaction(), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Once);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_RollbackTransactionAsync_When_AsyncHandlerThrows()
    {
        // Arrange & Act
        Func<Task> act = () => _mockSession.Object.TransactionWrapAsync(async () =>
        {
            await Task.CompletedTask;
            throw new InvalidOperationException("async error");
        });

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("async error");
        _mockSession.Verify(s => s.BeginTransaction(), Times.Once);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_CommitTransactionAsyncWithIsolationLevel_When_AsyncHandlerSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        await _mockSession.Object.TransactionWrapAsync(IsolationLevel.Serializable, async () =>
        {
            invoked = true;
            await Task.CompletedTask;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockSession.Verify(s => s.BeginTransaction(IsolationLevel.Serializable), Times.Once);
        _mockSession.Verify(s => s.CommitTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_RollbackTransactionAsyncWithIsolationLevel_When_AsyncHandlerThrows()
    {
        // Arrange & Act
        Func<Task> act = () => _mockSession.Object.TransactionWrapAsync(IsolationLevel.ReadCommitted, async () =>
        {
            await Task.CompletedTask;
            throw new InvalidOperationException("async error");
        });

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        _mockSession.Verify(s => s.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        _mockSession.Verify(s => s.RollbackTransaction(), Times.Once);
    }

    #endregion

    #region ITransaction.TransactionWrap

    [Fact]
    public void Should_CommitOnTransaction_When_TransactionWrapSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        _mockTransaction.Object.TransactionWrap(() =>
        {
            invoked = true;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockTransaction.Verify(t => t.BeginTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.CommitTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public void Should_RollbackOnTransaction_When_TransactionWrapThrows()
    {
        // Arrange & Act
        Action act = () => _mockTransaction.Object.TransactionWrap(() =>
        {
            throw new InvalidOperationException("tx error");
        });

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("tx error");
        _mockTransaction.Verify(t => t.BeginTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.RollbackTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.CommitTransaction(), Times.Never);
    }

    [Fact]
    public void Should_CommitOnTransactionWithIsolationLevel_When_TransactionWrapSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        _mockTransaction.Object.TransactionWrap(IsolationLevel.Serializable, () =>
        {
            invoked = true;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockTransaction.Verify(t => t.BeginTransaction(IsolationLevel.Serializable), Times.Once);
        _mockTransaction.Verify(t => t.CommitTransaction(), Times.Once);
    }

    [Fact]
    public void Should_RollbackOnTransactionWithIsolationLevel_When_TransactionWrapThrows()
    {
        // Arrange & Act
        Action act = () => _mockTransaction.Object.TransactionWrap(IsolationLevel.ReadCommitted, () =>
        {
            throw new InvalidOperationException("tx error");
        });

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _mockTransaction.Verify(t => t.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        _mockTransaction.Verify(t => t.RollbackTransaction(), Times.Once);
    }

    #endregion

    #region ITransaction.TransactionWrapAsync

    [Fact]
    public async Task Should_CommitOnTransactionAsync_When_AsyncHandlerSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        await _mockTransaction.Object.TransactionWrapAsync(async () =>
        {
            invoked = true;
            await Task.CompletedTask;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockTransaction.Verify(t => t.BeginTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.CommitTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.RollbackTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_RollbackOnTransactionAsync_When_AsyncHandlerThrows()
    {
        // Arrange & Act
        Func<Task> act = () => _mockTransaction.Object.TransactionWrapAsync(async () =>
        {
            await Task.CompletedTask;
            throw new InvalidOperationException("async tx error");
        });

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("async tx error");
        _mockTransaction.Verify(t => t.BeginTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.RollbackTransaction(), Times.Once);
        _mockTransaction.Verify(t => t.CommitTransaction(), Times.Never);
    }

    [Fact]
    public async Task Should_CommitOnTransactionAsyncWithIsolationLevel_When_AsyncHandlerSucceeds()
    {
        // Arrange
        var invoked = false;

        // Act
        await _mockTransaction.Object.TransactionWrapAsync(IsolationLevel.Serializable, async () =>
        {
            invoked = true;
            await Task.CompletedTask;
        });

        // Assert
        invoked.Should().BeTrue();
        _mockTransaction.Verify(t => t.BeginTransaction(IsolationLevel.Serializable), Times.Once);
        _mockTransaction.Verify(t => t.CommitTransaction(), Times.Once);
    }

    [Fact]
    public async Task Should_RollbackOnTransactionAsyncWithIsolationLevel_When_AsyncHandlerThrows()
    {
        // Arrange & Act
        Func<Task> act = () => _mockTransaction.Object.TransactionWrapAsync(IsolationLevel.ReadCommitted, async () =>
        {
            await Task.CompletedTask;
            throw new InvalidOperationException("async tx error");
        });

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
        _mockTransaction.Verify(t => t.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once);
        _mockTransaction.Verify(t => t.RollbackTransaction(), Times.Once);
    }

    #endregion
}
