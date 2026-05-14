using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit;

public class SqlMapperAsyncTests
{
    private readonly Mock<IDbSessionStore> _mockSessionStore;
    private readonly Mock<IDbSession> _mockSession;
    private readonly SmartSqlConfig _config;

    public SqlMapperAsyncTests()
    {
        _mockSessionStore = new Mock<IDbSessionStore>();
        _mockSession = new Mock<IDbSession>();

        var dbProvider = new DbProvider
        {
            Name = "SQLite",
            ParameterPrefix = "@"
        };

        _config = new SmartSqlConfig
        {
            Database = new Database { DbProvider = dbProvider },
            SessionStore = _mockSessionStore.Object
        };
    }

    private SqlMapper CreateMapper(IDbSession localSession = null)
    {
        _mockSessionStore.Setup(s => s.LocalSession).Returns(localSession);
        if (localSession == null)
        {
            _mockSessionStore.Setup(s => s.Open()).Returns(_mockSession.Object);
        }

        return new SqlMapper(_config);
    }

    #region ExecuteAsync

    [Fact]
    public async Task Should_ExecuteAsync_When_NoLocalSessionExists()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "DELETE FROM T_User WHERE Id=1" };

        _mockSession.Setup(s => s.ExecuteAsync(It.IsAny<AbstractRequestContext>())).ReturnsAsync(3);

        var result = await mapper.ExecuteAsync(requestContext);

        result.Should().Be(3);
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public async Task Should_ExecuteAsync_When_LocalSessionExists()
    {
        var mapper = CreateMapper(_mockSession.Object);
        var requestContext = new RequestContext { RealSql = "DELETE FROM T_User WHERE Id=1" };

        _mockSession.Setup(s => s.ExecuteAsync(It.IsAny<AbstractRequestContext>())).ReturnsAsync(5);

        var result = await mapper.ExecuteAsync(requestContext);

        result.Should().Be(5);
        _mockSessionStore.Verify(s => s.Open(), Times.Never);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Never);
    }

    [Fact]
    public async Task Should_DisposeSession_When_ExecuteAsyncThrows()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "BAD SQL" };

        _mockSession.Setup(s => s.ExecuteAsync(It.IsAny<AbstractRequestContext>()))
            .ThrowsAsync(new InvalidOperationException("SQL error"));

        var act = async () => await mapper.ExecuteAsync(requestContext);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    #endregion

    #region ExecuteScalarAsync

    [Fact]
    public async Task Should_ExecuteScalarAsync_When_NoLocalSessionExists()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "SELECT COUNT(*) FROM T_User" };

        _mockSession.Setup(s => s.ExecuteScalarAsync<long>(It.IsAny<AbstractRequestContext>())).ReturnsAsync(42L);

        var result = await mapper.ExecuteScalarAsync<long>(requestContext);

        result.Should().Be(42L);
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public async Task Should_ExecuteScalarAsync_When_LocalSessionExists()
    {
        var mapper = CreateMapper(_mockSession.Object);
        var requestContext = new RequestContext { RealSql = "SELECT COUNT(*) FROM T_User" };

        _mockSession.Setup(s => s.ExecuteScalarAsync<int>(It.IsAny<AbstractRequestContext>())).ReturnsAsync(99);

        var result = await mapper.ExecuteScalarAsync<int>(requestContext);

        result.Should().Be(99);
        _mockSessionStore.Verify(s => s.Open(), Times.Never);
    }

    [Fact]
    public async Task Should_DisposeSession_When_ExecuteScalarAsyncThrows()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "BAD SQL" };

        _mockSession.Setup(s => s.ExecuteScalarAsync<int>(It.IsAny<AbstractRequestContext>()))
            .ThrowsAsync(new InvalidOperationException("error"));

        var act = async () => await mapper.ExecuteScalarAsync<int>(requestContext);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    #endregion

    #region QueryAsync

    [Fact]
    public async Task Should_QueryAsync_When_NoLocalSessionExists()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "SELECT * FROM T_User" };
        var expected = new List<UserEntity> { new UserEntity { Id = 1, Name = "Test" } };

        _mockSession.Setup(s => s.QueryAsync<UserEntity>(It.IsAny<AbstractRequestContext>()))
            .ReturnsAsync(expected);

        var result = await mapper.QueryAsync<UserEntity>(requestContext);

        result.Should().BeEquivalentTo(expected);
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public async Task Should_QueryAsync_When_LocalSessionExists()
    {
        var mapper = CreateMapper(_mockSession.Object);
        var requestContext = new RequestContext { RealSql = "SELECT * FROM T_User" };
        var expected = new List<UserEntity>();

        _mockSession.Setup(s => s.QueryAsync<UserEntity>(It.IsAny<AbstractRequestContext>()))
            .ReturnsAsync(expected);

        var result = await mapper.QueryAsync<UserEntity>(requestContext);

        result.Should().BeEmpty();
        _mockSessionStore.Verify(s => s.Open(), Times.Never);
    }

    #endregion

    #region QuerySingleAsync

    [Fact]
    public async Task Should_QuerySingleAsync_When_NoLocalSessionExists()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "SELECT * FROM T_User WHERE Id=1" };
        var expected = new UserEntity { Id = 1, Name = "Test" };

        _mockSession.Setup(s => s.QuerySingleAsync<UserEntity>(It.IsAny<AbstractRequestContext>()))
            .ReturnsAsync(expected);

        var result = await mapper.QuerySingleAsync<UserEntity>(requestContext);

        result.Should().BeEquivalentTo(expected);
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public async Task Should_QuerySingleAsync_When_LocalSessionExists()
    {
        var mapper = CreateMapper(_mockSession.Object);
        var requestContext = new RequestContext { RealSql = "SELECT * FROM T_User WHERE Id=1" };

        _mockSession.Setup(s => s.QuerySingleAsync<UserEntity>(It.IsAny<AbstractRequestContext>()))
            .ReturnsAsync((UserEntity)null);

        var result = await mapper.QuerySingleAsync<UserEntity>(requestContext);

        result.Should().BeNull();
        _mockSessionStore.Verify(s => s.Open(), Times.Never);
    }

    #endregion

    #region GetDataTableAsync

    [Fact]
    public async Task Should_GetDataTableAsync_When_NoLocalSessionExists()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "SELECT * FROM T_User" };
        var expected = new DataTable();

        _mockSession.Setup(s => s.GetDataTableAsync(It.IsAny<AbstractRequestContext>()))
            .ReturnsAsync(expected);

        var result = await mapper.GetDataTableAsync(requestContext);

        result.Should().BeSameAs(expected);
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    #endregion

    #region GetDataSetAsync

    [Fact]
    public async Task Should_GetDataSetAsync_When_NoLocalSessionExists()
    {
        var mapper = CreateMapper();
        var requestContext = new RequestContext { RealSql = "SELECT * FROM T_User" };
        var expected = new DataSet();

        _mockSession.Setup(s => s.GetDataSetAsync(It.IsAny<AbstractRequestContext>()))
            .ReturnsAsync(expected);

        var result = await mapper.GetDataSetAsync(requestContext);

        result.Should().BeSameAs(expected);
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    #endregion

    private class UserEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
