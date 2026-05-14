using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Moq;
using SmartSql.Command;
using SmartSql.Configuration;
using SmartSql.Data;
using SmartSql.DataSource;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit.Command;

public class CommandExecuterTests
{
    private readonly CommandExecuter _commandExecuter;

    public CommandExecuterTests()
    {
        _commandExecuter = new CommandExecuter(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<CommandExecuter>.Instance);
    }

    private static (ExecutionContext context, SqliteConnection connection) CreateTestContext(
        string realSql = "SELECT 1",
        Action<SqlParameterCollection> configureParams = null,
        int? commandTimeout = null)
    {
        var dbProvider = DbProviderManager.SQLITE_DBPROVIDER;
        // Ensure Factory is resolved (lazy-loaded in DbProviderManager)
        DbProviderManager.Instance.TryInit(ref dbProvider);

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
            }
        };

        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var mockSession = new Mock<IDbSession>();
        mockSession.SetupGet(s => s.Connection).Returns(connection);
        mockSession.SetupGet(s => s.Transaction).Returns((DbTransaction)null);
        mockSession.SetupGet(s => s.SmartSqlConfig).Returns(config);
        mockSession.Setup(s => s.Open()).Callback(() => { });
        mockSession.Setup(s => s.OpenAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.CompletedTask);

        var sqlParams = new SqlParameterCollection();
        configureParams?.Invoke(sqlParams);

        // Create RequestContext with a dummy Request object so SetupParameters works
        var requestContext = new RequestContext
        {
            CommandType = CommandType.Text,
            RealSql = realSql,
            CommandTimeout = commandTimeout,
            Scope = "TestScope",
            SqlId = "GetOne",
            Request = new { }
        };

        var executionContext = new ExecutionContext
        {
            SmartSqlConfig = config,
            DbSession = mockSession.Object,
            Request = requestContext
        };
        requestContext.ExecutionContext = executionContext;

        // Now manually set up the parameters collection
        requestContext.SetupParameters();

        // Add any extra db parameters from the test
        foreach (var dbParam in sqlParams.DbParameters.Values)
        {
            requestContext.Parameters.DbParameters.Add(dbParam.ParameterName, dbParam);
        }

        return (executionContext, connection);
    }

    #region Constructor

    [Fact]
    public void Should_CreateInstance_When_CalledWithLogger()
    {
        // Act
        var executer = new CommandExecuter(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<CommandExecuter>.Instance);

        // Assert
        executer.Should().NotBeNull();
    }

    [Fact]
    public void Should_ImplementICommandExecuter_When_Created()
    {
        // Act
        var executer = new CommandExecuter(
            Microsoft.Extensions.Logging.Abstractions.NullLogger<CommandExecuter>.Instance);

        // Assert
        executer.Should().BeAssignableTo<ICommandExecuter>();
    }

    #endregion

    #region ExecuteNonQuery

    [Fact]
    public void Should_ExecuteNonQuery_When_SqlIsValid()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = _commandExecuter.ExecuteNonQuery(context);

            // Assert
            result.Should().Be(-1); // SELECT 1 returns -1 for ExecuteNonQuery
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_ExecuteNonQueryAsync_When_SqlIsValid()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = await _commandExecuter.ExecuteNonQueryAsync(context);

            // Assert
            result.Should().Be(-1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_ExecuteNonQueryAsync_When_CancellationTokenProvided()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = await _commandExecuter.ExecuteNonQueryAsync(context, CancellationToken.None);

            // Assert
            result.Should().Be(-1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    #endregion

    #region ExecuteScalar

    [Fact]
    public void Should_ExecuteScalar_When_SqlReturnsSingleValue()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = _commandExecuter.ExecuteScalar(context);

            // Assert
            result.Should().NotBeNull();
            Convert.ToInt32(result).Should().Be(1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_ExecuteScalarAsync_When_SqlReturnsSingleValue()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = await _commandExecuter.ExecuteScalarAsync(context);

            // Assert
            result.Should().NotBeNull();
            Convert.ToInt32(result).Should().Be(1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_ExecuteScalarAsync_When_CancellationTokenProvided()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = await _commandExecuter.ExecuteScalarAsync(context, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            Convert.ToInt32(result).Should().Be(1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    #endregion

    #region ExecuteReader

    [Fact]
    public void Should_ExecuteReader_When_SqlIsValid()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = _commandExecuter.ExecuteReader(context);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DataReaderWrapper>();
            result.Dispose();
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_ExecuteReaderAsync_When_SqlIsValid()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = await _commandExecuter.ExecuteReaderAsync(context);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DataReaderWrapper>();
            result.Dispose();
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_ExecuteReaderAsync_When_CancellationTokenProvided()
    {
        // Arrange
        var (context, connection) = CreateTestContext();

        try
        {
            // Act
            var result = await _commandExecuter.ExecuteReaderAsync(context, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<DataReaderWrapper>();
            result.Dispose();
        }
        finally
        {
            connection.Dispose();
        }
    }

    #endregion

    #region DbCommandCreated Event

    [Fact]
    public void Should_RaiseDbCommandCreated_When_ExecuteNonQueryCalled()
    {
        // Arrange
        var (context, connection) = CreateTestContext();
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteNonQuery(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.Should().NotBeNull();
            capturedArgs.DbCommand.CommandText.Should().Be("SELECT 1");
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_RaiseDbCommandCreated_When_ExecuteScalarCalled()
    {
        // Arrange
        var (context, connection) = CreateTestContext();
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteScalar(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.Should().NotBeNull();
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_RaiseDbCommandCreated_When_ExecuteReaderCalled()
    {
        // Arrange
        var (context, connection) = CreateTestContext();
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            using var reader = _commandExecuter.ExecuteReader(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.Should().NotBeNull();
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_NotRaiseDbCommandCreated_When_NoSubscribers()
    {
        // Arrange
        var (context, connection) = CreateTestContext();
        // No subscribers - should not throw

        try
        {
            // Act
            var result = _commandExecuter.ExecuteNonQuery(context);

            // Assert - no exception thrown
            result.Should().Be(-1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_SetCommandTimeout_When_RequestHasTimeout()
    {
        // Arrange
        var (context, connection) = CreateTestContext(commandTimeout: 30);
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteNonQuery(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.CommandTimeout.Should().Be(30);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_UseDefaultTimeout_When_RequestHasNoTimeout()
    {
        // Arrange
        var (context, connection) = CreateTestContext();
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteNonQuery(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            // Default CommandTimeout is 30 seconds in .NET
            capturedArgs.DbCommand.CommandTimeout.Should().Be(30);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_AddDbParameters_When_RequestHasParameters()
    {
        // Arrange
        var (context, connection) = CreateTestContext(
            realSql: "SELECT @Id",
            configureParams: p =>
            {
                p.DbParameters.Add("@Id", new SqliteParameter("@Id", 42));
            });

        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteNonQuery(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.Parameters.Count.Should().BeGreaterThanOrEqualTo(1);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_SetCommandType_When_RequestSpecifiesType()
    {
        // Arrange
        var (context, connection) = CreateTestContext();
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteNonQuery(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.CommandType.Should().Be(CommandType.Text);
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_SetCommandText_When_RequestHasRealSql()
    {
        // Arrange
        var (context, connection) = CreateTestContext(realSql: "SELECT 42 AS Value");
        DbCommandCreatedEventArgs capturedArgs = null;
        _commandExecuter.DbCommandCreated += (_, args) => capturedArgs = args;

        try
        {
            // Act
            _commandExecuter.ExecuteNonQuery(context);

            // Assert
            capturedArgs.Should().NotBeNull();
            capturedArgs.DbCommand.CommandText.Should().Be("SELECT 42 AS Value");
        }
        finally
        {
            connection.Dispose();
        }
    }

    #endregion

    #region Error Handling

    [Fact]
    public void Should_PropagateException_When_DbCommandFails()
    {
        // Arrange
        var (context, connection) = CreateTestContext(realSql: "INVALID SQL STATEMENT!!");

        try
        {
            // Act
            var act = () => _commandExecuter.ExecuteNonQuery(context);

            // Assert
            act.Should().Throw<SqliteException>();
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public async Task Should_PropagateExceptionAsync_When_DbCommandFails()
    {
        // Arrange
        var (context, connection) = CreateTestContext(realSql: "INVALID SQL STATEMENT!!");

        try
        {
            // Act
            var act = () => _commandExecuter.ExecuteNonQueryAsync(context);

            // Assert
            await act.Should().ThrowAsync<SqliteException>();
        }
        finally
        {
            connection.Dispose();
        }
    }

    [Fact]
    public void Should_PropagateException_When_ScalarCommandFails()
    {
        // Arrange
        var (context, connection) = CreateTestContext(realSql: "INVALID SQL STATEMENT!!");

        try
        {
            // Act
            var act = () => _commandExecuter.ExecuteScalar(context);

            // Assert
            act.Should().Throw<SqliteException>();
        }
        finally
        {
            connection.Dispose();
        }
    }

    #endregion
}
