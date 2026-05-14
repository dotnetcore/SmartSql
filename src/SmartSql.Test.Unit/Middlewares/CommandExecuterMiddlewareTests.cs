using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql;
using SmartSql.Command;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Middlewares;
using Xunit;
using Microsoft.Data.Sqlite;

namespace SmartSql.Test.Unit.Middlewares;

public class CommandExecuterMiddlewareTests
{
    private static CommandExecuterMiddleware CreateMiddleware(
        out Mock<ICommandExecuter> mockExecuter,
        out SmartSqlConfig config)
    {
        mockExecuter = new Mock<ICommandExecuter>();

        var dbProvider = new DbProvider
        {
            Name = "SQLite",
            ParameterPrefix = "@",
            Factory = SqliteFactory.Instance
        };

        config = new SmartSqlConfig
        {
            Database = new Database { DbProvider = dbProvider }
        };

        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        var middleware = new CommandExecuterMiddleware();
        middleware.SetupSmartSql(builder);

        var field = typeof(CommandExecuterMiddleware).GetField("_commandExecuter",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(middleware, mockExecuter.Object);

        return middleware;
    }

    private static ExecutionContext CreateContext(
        ExecutionType type,
        ResultContext result = null,
        AbstractRequestContext request = null)
    {
        var req = request ?? new RequestContext<object>();
        req.ExecutionType = type;

        return new ExecutionContext
        {
            Request = req,
            Result = result ?? new SingleResultContext<int>(),
            SmartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { ParameterPrefix = "@" }
                }
            },
            DbSession = new Mock<IDbSession>().Object
        };
    }

    [Fact]
    public void Should_HaveOrder500()
    {
        var middleware = new CommandExecuterMiddleware();

        middleware.Order.Should().Be(500);
    }

    [Fact]
    public void Should_ExecuteNonQuery_When_ExecutionTypeIsExecute()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext(ExecutionType.Execute, result);

        mockExecuter.Setup(e => e.ExecuteNonQuery(context)).Returns(42);

        middleware.Invoke<int>(context);

        result.Data.Should().Be(42);
        mockExecuter.Verify(e => e.ExecuteNonQuery(context), Times.Once);
    }

    [Fact]
    public async Task Should_ExecuteNonQueryAsync_When_ExecutionTypeIsExecute()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext(ExecutionType.Execute, result);

        mockExecuter.Setup(e => e.ExecuteNonQueryAsync(context)).ReturnsAsync(99);

        await middleware.InvokeAsync<int>(context);

        result.Data.Should().Be(99);
        mockExecuter.Verify(e => e.ExecuteNonQueryAsync(context), Times.Once);
    }

    [Fact]
    public void Should_SetDefault_When_ExecuteScalarReturnsNull()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<string>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalar(context)).Returns(DBNull.Value);

        middleware.Invoke<string>(context);

        result.Data.Should().BeNull();
    }

    [Fact]
    public void Should_SetValue_When_ExecuteScalarReturnsCompatibleType()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalar(context)).Returns(42);

        middleware.Invoke<int>(context);

        result.Data.Should().Be(42);
    }

    [Fact]
    public void Should_ConvertValue_When_ExecuteScalarReturnsDifferentType()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalar(context)).Returns(42L);

        middleware.Invoke<int>(context);

        result.Data.Should().Be(42);
    }

    [Fact]
    public void Should_ParseGuid_When_ExecuteScalarReturnsStringAndResultIsGuid()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var guidValue = Guid.NewGuid();
        var result = new SingleResultContext<Guid>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalar(context)).Returns(guidValue.ToString());

        middleware.Invoke<Guid>(context);

        result.Data.Should().Be(guidValue);
    }

    [Fact]
    public void Should_ConvertToEnum_When_ExecuteScalarReturnsIntForEnumType()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<DayOfWeek>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalar(context)).Returns(3);

        middleware.Invoke<DayOfWeek>(context);

        result.Data.Should().Be(DayOfWeek.Wednesday);
    }

    [Fact]
    public void Should_SetNullableUnderlyingType_When_ExecuteScalarReturnsDbNull()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int?>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalar(context)).Returns(DBNull.Value);

        middleware.Invoke<int?>(context);

        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task Should_ExecuteScalarAsync_When_ExecutionTypeIsExecuteScalar()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext(ExecutionType.ExecuteScalar, result);

        mockExecuter.Setup(e => e.ExecuteScalarAsync(context)).ReturnsAsync(77);

        await middleware.InvokeAsync<int>(context);

        result.Data.Should().Be(77);
    }

    [Fact]
    public void Should_SetDataTable_When_ExecutionTypeIsGetDataTable()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var dataTable = new DataTable();
        var result = new SingleResultContext<DataTable>();
        var context = CreateContext(ExecutionType.GetDataTable, result);

        mockExecuter.Setup(e => e.GetDateTable(context)).Returns(dataTable);

        middleware.Invoke<DataTable>(context);

        result.Data.Should().BeSameAs(dataTable);
    }

    [Fact]
    public void Should_SetDataSet_When_ExecutionTypeIsGetDataSet()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var dataSet = new DataSet();
        var result = new SingleResultContext<DataSet>();
        var context = CreateContext(ExecutionType.GetDataSet, result);

        mockExecuter.Setup(e => e.GetDateSet(context)).Returns(dataSet);

        middleware.Invoke<DataSet>(context);

        result.Data.Should().BeSameAs(dataSet);
    }

    [Fact]
    public async Task Should_SetDataTableAsync_When_ExecutionTypeIsGetDataTable()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var dataTable = new DataTable();
        var result = new SingleResultContext<DataTable>();
        var context = CreateContext(ExecutionType.GetDataTable, result);

        mockExecuter.Setup(e => e.GetDateTableAsync(context)).ReturnsAsync(dataTable);

        await middleware.InvokeAsync<DataTable>(context);

        result.Data.Should().BeSameAs(dataTable);
    }

    [Fact]
    public async Task Should_SetDataSetAsync_When_ExecutionTypeIsGetDataSet()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var dataSet = new DataSet();
        var result = new SingleResultContext<DataSet>();
        var context = CreateContext(ExecutionType.GetDataSet, result);

        mockExecuter.Setup(e => e.GetDateSetAsync(context)).ReturnsAsync(dataSet);

        await middleware.InvokeAsync<DataSet>(context);

        result.Data.Should().BeSameAs(dataSet);
    }

    [Fact]
    public void Should_ThrowArgumentOutOfRange_When_ExecutionTypeIsUnknown()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext((ExecutionType)999, result);

        var act = () => middleware.Invoke<int>(context);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task Should_ThrowArgumentOutOfRange_When_ExecutionTypeIsUnknownAsync()
    {
        var middleware = CreateMiddleware(out var mockExecuter, out var config);
        var result = new SingleResultContext<int>();
        var context = CreateContext((ExecutionType)999, result);

        var act = () => middleware.InvokeAsync<int>(context);

        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}
