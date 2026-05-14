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

public class DataSourceFilterMiddlewareTests
{
    private static (DataSourceFilterMiddleware middleware, Mock<IDataSourceFilter> mockFilter, Mock<IDbSession> mockSession, SmartSqlConfig config)
        CreateMiddleware()
    {
        var mockFilter = new Mock<IDataSourceFilter>();
        var mockSession = new Mock<IDbSession>();

        var config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = new DbProvider { ParameterPrefix = "@" }
            },
            DataSourceFilter = mockFilter.Object
        };

        var builder = new SmartSqlBuilder();
        var configProp = typeof(SmartSqlBuilder).GetProperty("SmartSqlConfig");
        configProp.SetValue(builder, config);

        var middleware = new DataSourceFilterMiddleware();
        middleware.SetupSmartSql(builder);

        return (middleware, mockFilter, mockSession, config);
    }

    private static ExecutionContext CreateContext(AbstractRequestContext request = null)
    {
        return new ExecutionContext
        {
            Request = request ?? new RequestContext<object>(),
            Result = new SingleResultContext<int>(),
            SmartSqlConfig = new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = new DbProvider { ParameterPrefix = "@" }
                }
            }
        };
    }

    [Fact]
    public void Should_HaveOrder400()
    {
        var middleware = new DataSourceFilterMiddleware();

        middleware.Order.Should().Be(400);
    }

    [Fact]
    public void Should_SelectDataSource_When_SessionHasNoDataSource()
    {
        var (middleware, mockFilter, mockSession, _) = CreateMiddleware();
        var dataSource = new Mock<AbstractDataSource>().Object;
        mockFilter.Setup(f => f.Elect(It.IsAny<AbstractRequestContext>())).Returns(dataSource);
        mockSession.Setup(s => s.DataSource).Returns((AbstractDataSource)null);
        mockSession.Setup(s => s.SetDataSource(It.IsAny<AbstractDataSource>()));

        var context = CreateContext();
        context.DbSession = mockSession.Object;

        middleware.Invoke<int>(context);

        mockFilter.Verify(f => f.Elect(It.IsAny<AbstractRequestContext>()), Times.Once);
        mockSession.Verify(s => s.SetDataSource(dataSource), Times.Once);
    }

    [Fact]
    public void Should_NotSelectDataSource_When_SessionAlreadyHasDataSource()
    {
        var (middleware, mockFilter, mockSession, _) = CreateMiddleware();
        var existingDataSource = new Mock<AbstractDataSource>().Object;
        mockSession.Setup(s => s.DataSource).Returns(existingDataSource);

        var context = CreateContext();
        context.DbSession = mockSession.Object;

        middleware.Invoke<int>(context);

        mockFilter.Verify(f => f.Elect(It.IsAny<AbstractRequestContext>()), Times.Never);
    }

    [Fact]
    public void Should_InvokeNext_When_DataSourceSelected()
    {
        var (middleware, mockFilter, mockSession, _) = CreateMiddleware();
        var dataSource = new Mock<AbstractDataSource>().Object;
        mockFilter.Setup(f => f.Elect(It.IsAny<AbstractRequestContext>())).Returns(dataSource);
        mockSession.Setup(s => s.DataSource).Returns((AbstractDataSource)null);
        mockSession.Setup(s => s.SetDataSource(It.IsAny<AbstractDataSource>()));

        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<int>(It.IsAny<ExecutionContext>()))
            .Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        var context = CreateContext();
        context.DbSession = mockSession.Object;

        middleware.Invoke<int>(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_SelectDataSourceAsync_When_SessionHasNoDataSource()
    {
        var (middleware, mockFilter, mockSession, _) = CreateMiddleware();
        var dataSource = new Mock<AbstractDataSource>().Object;
        mockFilter.Setup(f => f.Elect(It.IsAny<AbstractRequestContext>())).Returns(dataSource);
        mockSession.Setup(s => s.DataSource).Returns((AbstractDataSource)null);
        mockSession.Setup(s => s.SetDataSource(It.IsAny<AbstractDataSource>()));

        var context = CreateContext();
        context.DbSession = mockSession.Object;

        await middleware.InvokeAsync<int>(context);

        mockFilter.Verify(f => f.Elect(It.IsAny<AbstractRequestContext>()), Times.Once);
        mockSession.Verify(s => s.SetDataSource(dataSource), Times.Once);
    }

    [Fact]
    public async Task Should_InvokeNextAsync_When_DataSourceSelected()
    {
        var (middleware, mockFilter, mockSession, _) = CreateMiddleware();
        var dataSource = new Mock<AbstractDataSource>().Object;
        mockFilter.Setup(f => f.Elect(It.IsAny<AbstractRequestContext>())).Returns(dataSource);
        mockSession.Setup(s => s.DataSource).Returns((AbstractDataSource)null);
        mockSession.Setup(s => s.SetDataSource(It.IsAny<AbstractDataSource>()));

        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<int>(It.IsAny<ExecutionContext>()))
            .Callback(() => nextCalled = true)
            .Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        var context = CreateContext();
        context.DbSession = mockSession.Object;

        await middleware.InvokeAsync<int>(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_NotSelectDataSourceAsync_When_SessionAlreadyHasDataSource()
    {
        var (middleware, mockFilter, mockSession, _) = CreateMiddleware();
        var existingDataSource = new Mock<AbstractDataSource>().Object;
        mockSession.Setup(s => s.DataSource).Returns(existingDataSource);

        var context = CreateContext();
        context.DbSession = mockSession.Object;

        await middleware.InvokeAsync<int>(context);

        mockFilter.Verify(f => f.Elect(It.IsAny<AbstractRequestContext>()), Times.Never);
    }
}
