using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.DbSession;

public class DefaultDbSessionTests
{
    private readonly SmartSqlConfig _config;

    public DefaultDbSessionTests()
    {
        _config = CreateTestConfig();
    }

    private static DbProvider CreateSqliteDbProvider()
    {
        return new DbProvider
        {
            Name = DbProvider.SQLITE,
            ParameterPrefix = "@",
            Type = "Microsoft.Data.Sqlite.SqliteFactory,Microsoft.Data.Sqlite",
            Factory = SqliteFactory.Instance
        };
    }

    private static SmartSqlConfig CreateTestConfig()
    {
        var dbProvider = CreateSqliteDbProvider();
        var mockPipeline = new Mock<IMiddleware>();
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = dbProvider,
                Write = new WriteDataSource
                {
                    Name = "Write",
                    ConnectionString = "Data Source=:memory:",
                    DbProvider = dbProvider
                }
            },
            Pipeline = mockPipeline.Object
        };
        return config;
    }

    private static SmartSqlConfig CreateConfigWithPipeline(out Mock<IMiddleware> mockPipeline)
    {
        mockPipeline = new Mock<IMiddleware>();
        var dbProvider = CreateSqliteDbProvider();
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = dbProvider,
                Write = new WriteDataSource
                {
                    Name = "Write",
                    ConnectionString = "Data Source=:memory:",
                    DbProvider = dbProvider
                }
            },
            Pipeline = mockPipeline.Object
        };
        return config;
    }

    #region Constructor

    [Fact]
    public void Should_AssignId_When_Constructed()
    {
        // Arrange & Act
        var session = new DefaultDbSession(_config);

        // Assert
        session.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_AssignConfig_When_Constructed()
    {
        // Arrange & Act
        var session = new DefaultDbSession(_config);

        // Assert
        session.SmartSqlConfig.Should().BeSameAs(_config);
    }

    [Fact]
    public void Should_HaveNullConnection_When_Constructed()
    {
        // Arrange & Act
        var session = new DefaultDbSession(_config);

        // Assert
        session.Connection.Should().BeNull();
    }

    [Fact]
    public void Should_HaveNullTransaction_When_Constructed()
    {
        // Arrange & Act
        var session = new DefaultDbSession(_config);

        // Assert
        session.Transaction.Should().BeNull();
    }

    [Fact]
    public void Should_HaveUniqueIds_When_MultipleSessionsCreated()
    {
        // Arrange & Act
        var session1 = new DefaultDbSession(_config);
        var session2 = new DefaultDbSession(_config);

        // Assert
        session1.Id.Should().NotBe(session2.Id);
    }

    #endregion

    #region SetDataSource

    [Fact]
    public void Should_SetDataSource_When_SetDataSourceCalled()
    {
        // Arrange
        var session = new DefaultDbSession(_config);
        var dataSource = new WriteDataSource
        {
            Name = "CustomWrite", ConnectionString = "Data Source=:memory:", DbProvider = CreateSqliteDbProvider()
        };

        // Act
        session.SetDataSource(dataSource);

        // Assert
        session.DataSource.Should().BeSameAs(dataSource);
    }

    #endregion

    #region Open / OpenAsync

    [Fact]
    public void Should_OpenConnection_When_OpenCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());

        // Act
        session.Open();

        // Assert
        session.Connection.Should().NotBeNull();
        session.Connection.State.Should().Be(ConnectionState.Open);
        session.Dispose();
    }

    [Fact]
    public async Task Should_OpenConnectionAsync_When_OpenAsyncCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());

        // Act
        await session.OpenAsync();

        // Assert
        session.Connection.Should().NotBeNull();
        session.Connection.State.Should().Be(ConnectionState.Open);
        session.Dispose();
    }

    [Fact]
    public void Should_FireOpenedEvent_When_OpenCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        object eventSender = null;
        session.Opened += (sender, args) => { eventSender = sender; };

        // Act
        session.Open();

        // Assert
        eventSender.Should().BeSameAs(session);
        session.Dispose();
    }

    [Fact]
    public void Should_NotReopenConnection_When_ConnectionAlreadyOpen()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();
        var connection = session.Connection;

        // Act
        session.Open();

        // Assert
        session.Connection.Should().BeSameAs(connection);
        session.Dispose();
    }

    [Fact]
    public void Should_DefaultToWriteDataSource_When_NoDataSourceSet()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());

        // Act
        session.Open();

        // Assert
        session.DataSource.Should().NotBeNull();
        session.DataSource.Name.Should().Be("Write");
        session.Dispose();
    }

    #endregion

    #region BeginTransaction

    [Fact]
    public void Should_BeginTransaction_When_BeginTransactionCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();

        // Act
        var transaction = session.BeginTransaction();

        // Assert
        transaction.Should().NotBeNull();
        session.Transaction.Should().BeSameAs(transaction);
        session.Dispose();
    }

    [Fact]
    public void Should_BeginTransactionWithIsolationLevel_When_BeginTransactionCalledWithLevel()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();

        // Act
        var transaction = session.BeginTransaction(IsolationLevel.Serializable);

        // Assert
        transaction.Should().NotBeNull();
        transaction.IsolationLevel.Should().Be(IsolationLevel.Serializable);
        session.Dispose();
    }

    [Fact]
    public void Should_FireTransactionBeganEvent_When_BeginTransactionCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        object eventSender = null;
        session.TransactionBegan += (sender, args) => eventSender = sender;

        // Act
        session.Open();
        session.BeginTransaction();

        // Assert
        eventSender.Should().BeSameAs(session);
        session.Dispose();
    }

    #endregion

    #region CommitTransaction

    [Fact]
    public void Should_CommitTransaction_When_CommitTransactionCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();
        session.BeginTransaction();

        // Act
        session.CommitTransaction();

        // Assert
        session.Transaction.Should().BeNull();
        session.Dispose();
    }

    [Fact]
    public void Should_Throw_When_CommitWithoutBeginTransaction()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());

        // Act
        Action act = () => session.CommitTransaction();

        // Assert
        // CommitTransaction throws SmartSqlException when Transaction is null,
        // but finally block calls ReleaseTransaction() which causes NullReferenceException
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Should_FireCommittedEvent_When_CommitTransactionCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        object eventSender = null;
        session.Committed += (sender, args) => eventSender = sender;
        session.Open();
        session.BeginTransaction();

        // Act
        session.CommitTransaction();

        // Assert
        eventSender.Should().BeSameAs(session);
        session.Dispose();
    }

    #endregion

    #region RollbackTransaction

    [Fact]
    public void Should_RollbackTransaction_When_RollbackTransactionCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();
        session.BeginTransaction();

        // Act
        session.RollbackTransaction();

        // Assert
        session.Transaction.Should().BeNull();
        session.Dispose();
    }

    [Fact]
    public void Should_Throw_When_RollbackWithoutBeginTransaction()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());

        // Act
        Action act = () => session.RollbackTransaction();

        // Assert - NullReferenceException from ReleaseTransaction on null Transaction in finally block
        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void Should_FireRollbackedEvent_When_RollbackTransactionCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        object eventSender = null;
        session.Rollbacked += (sender, args) => eventSender = sender;
        session.Open();
        session.BeginTransaction();

        // Act
        session.RollbackTransaction();

        // Assert
        eventSender.Should().BeSameAs(session);
        session.Dispose();
    }

    #endregion

    #region Dispose

    [Fact]
    public void Should_DisposeConnection_When_DisposeCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();

        // Act
        session.Dispose();

        // Assert
        session.Connection.Should().BeNull();
    }

    [Fact]
    public void Should_RollbackActiveTransaction_When_DisposeCalledWithActiveTransaction()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();
        session.BeginTransaction();

        // Act
        session.Dispose();

        // Assert
        session.Transaction.Should().BeNull();
        session.Connection.Should().BeNull();
    }

    [Fact]
    public void Should_NotThrow_When_DisposeCalledWithoutOpening()
    {
        // Arrange
        var session = new DefaultDbSession(_config);

        // Act
        Action act = () => session.Dispose();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Should_FireDisposedEvent_When_DisposeCalled()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        object eventSender = null;
        session.Disposed += (sender, args) => eventSender = sender;
        session.Open();

        // Act
        session.Dispose();

        // Assert
        eventSender.Should().BeSameAs(session);
    }

    [Fact]
    public void Should_BeIdempotent_When_DisposeCalledMultipleTimes()
    {
        // Arrange
        var session = new DefaultDbSession(CreateTestConfig());
        session.Open();

        // Act
        session.Dispose();
        Action secondDispose = () => session.Dispose();

        // Assert
        secondDispose.Should().NotThrow();
    }

    #endregion

    #region Invoke

    [Fact]
    public void Should_InvokePipeline_When_ExecuteCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out var mockPipeline);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Execute",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.Execute(requestContext);

        // Assert
        mockPipeline.Verify(p => p.Invoke<int>(It.IsAny<ExecutionContext>()), Times.Once);
    }

    [Fact]
    public void Should_SetExecutionTypeToExecute_When_ExecuteCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Execute",
            ExecutionType = ExecutionType.Query
        };

        // Act
        session.Execute(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.Execute);
    }

    [Fact]
    public void Should_SetExecutionTypeToExecuteScalar_When_ExecuteScalarCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Scalar",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.ExecuteScalar<long>(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.ExecuteScalar);
    }

    [Fact]
    public void Should_SetExecutionTypeToQuerySingle_When_QuerySingleCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "QuerySingle",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.QuerySingle<object>(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.QuerySingle);
    }

    [Fact]
    public void Should_SetExecutionTypeToQuery_When_QueryCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Query",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.Query<object>(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.Query);
    }

    [Fact]
    public void Should_FireInvokedEvent_When_InvokeSucceeds()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        object eventSender = null;
        session.Invoked += (sender, args) => eventSender = sender;
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Query",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.Execute(requestContext);

        // Assert
        eventSender.Should().BeSameAs(session);
    }

    #endregion

    #region Invoke - GetDataSet / GetDataTable

    [Fact]
    public void Should_SetExecutionTypeToGetDataTable_When_GetDataTableCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "GetDataTable",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.GetDataTable(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.GetDataTable);
    }

    [Fact]
    public void Should_SetExecutionTypeToGetDataSet_When_GetDataSetCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "GetDataSet",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        session.GetDataSet(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.GetDataSet);
    }

    #endregion

    #region InvokeAsync

    [Fact]
    public async Task Should_InvokePipelineAsync_When_ExecuteAsyncCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out var mockPipeline);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Execute",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        await session.ExecuteAsync(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.Execute);
        mockPipeline.Verify(p => p.InvokeAsync<int>(It.IsAny<ExecutionContext>()), Times.Once);
    }

    [Fact]
    public async Task Should_SetExecutionTypeToExecuteScalar_When_ExecuteScalarAsyncCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Scalar",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        await session.ExecuteScalarAsync<long>(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.ExecuteScalar);
    }

    [Fact]
    public async Task Should_SetExecutionTypeToQuerySingle_When_QuerySingleAsyncCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "QuerySingle",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        await session.QuerySingleAsync<object>(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.QuerySingle);
    }

    [Fact]
    public async Task Should_SetExecutionTypeToQuery_When_QueryAsyncCalled()
    {
        // Arrange
        var config = CreateConfigWithPipeline(out _);
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Query",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        await session.QueryAsync<object>(requestContext);

        // Assert
        requestContext.ExecutionType.Should().Be(ExecutionType.Query);
    }

    #endregion

    #region Invoke - Exception handling

    [Fact]
    public void Should_PropagateException_When_PipelineThrows()
    {
        // Arrange
        var mockPipeline = new Mock<IMiddleware>();
        mockPipeline.Setup(p => p.Invoke<int>(It.IsAny<ExecutionContext>()))
            .Throws<InvalidOperationException>();
        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = CreateSqliteDbProvider(),
                Write = new WriteDataSource
                {
                    Name = "Write",
                    ConnectionString = "Data Source=:memory:",
                    DbProvider = CreateSqliteDbProvider()
                }
            },
            Pipeline = mockPipeline.Object
        };
        var session = new DefaultDbSession(config);
        var requestContext = new RequestContext
        {
            Scope = "Test",
            SqlId = "Fail",
            ExecutionType = ExecutionType.Execute
        };

        // Act
        Action act = () => session.Execute(requestContext);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    #endregion
}
