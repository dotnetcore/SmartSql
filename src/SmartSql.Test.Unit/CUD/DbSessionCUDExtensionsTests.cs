using System;
using System.Collections.Generic;
using System.Data;
using FluentAssertions;
using Moq;
using SmartSql;
using SmartSql.Annotations;
using SmartSql.Configuration;
using SmartSql.CUD;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.CUD;

public class DbSessionCUDExtensionsTests
{
    private readonly Mock<IDbSession> _mockSession;
    private readonly SmartSqlConfig _config;

    public DbSessionCUDExtensionsTests()
    {
        _mockSession = new Mock<IDbSession>();

        _config = new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = new DbProvider
                {
                    Name = "SQLite",
                    ParameterPrefix = "@"
                }
            }
        };

        _mockSession.Setup(s => s.SmartSqlConfig).Returns(_config);
    }

    #region Insert

    [Fact]
    public void Should_Insert_When_EntityIsValid()
    {
        var entity = new AllPrimitive { Boolean = true, Int64 = 100 };
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(1);

        var result = _mockSession.Object.Insert(entity);

        result.Should().Be(1);
        _mockSession.Verify(s => s.Execute(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.INSERT)), Times.Once);
    }

    [Fact]
    public void Should_InsertWithId_When_EntityIsValid()
    {
        var entity = new AllPrimitive { Boolean = true, Int64 = 200 };
        _mockSession.Setup(s => s.ExecuteScalar<long>(It.IsAny<AbstractRequestContext>())).Returns(1L);

        var result = _mockSession.Object.Insert<AllPrimitive, long>(entity);

        result.Should().Be(1L);
        entity.Id.Should().Be(1L);
        _mockSession.Verify(s => s.ExecuteScalar<long>(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.INSERT_RETURN_ID)), Times.Once);
    }

    #endregion

    #region GetById

    [Fact]
    public void Should_GetById_When_IdIsValid()
    {
        var expected = new AllPrimitive { Id = 1, Boolean = true, Int64 = 100 };
        _mockSession.Setup(s => s.QuerySingle<AllPrimitive>(It.IsAny<AbstractRequestContext>())).Returns(expected);

        var result = _mockSession.Object.GetById<AllPrimitive, long>(1L);

        result.Should().NotBeNull();
        result.Id.Should().Be(1L);
        _mockSession.Verify(s => s.QuerySingle<AllPrimitive>(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.GET_BY_ID)), Times.Once);
    }

    [Fact]
    public void Should_GetByIdWithTrack_When_EnablePropertyChangedTrackIsTrue()
    {
        var expected = new AllPrimitive { Id = 1, Boolean = true };
        _mockSession.Setup(s => s.QuerySingle<AllPrimitive>(It.IsAny<AbstractRequestContext>())).Returns(expected);

        var result = _mockSession.Object.GetById<AllPrimitive, long>(1L, true);

        result.Should().NotBeNull();
        _mockSession.Verify(s => s.QuerySingle<AllPrimitive>(It.Is<AbstractRequestContext>(ctx =>
            ctx.EnablePropertyChangedTrack == true)), Times.Once);
    }

    #endregion

    #region Delete

    [Fact]
    public void Should_DeleteById_When_IdIsValid()
    {
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(1);

        var result = _mockSession.Object.DeleteById<AllPrimitive, long>(1L);

        result.Should().Be(1);
        _mockSession.Verify(s => s.Execute(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.DELETE_BY_ID)), Times.Once);
    }

    [Fact]
    public void Should_DeleteMany_When_IdsAreValid()
    {
        var ids = new List<long> { 1, 2, 3 };
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(3);

        var result = _mockSession.Object.DeleteMany<AllPrimitive, long>(ids);

        result.Should().Be(3);
        _mockSession.Verify(s => s.Execute(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.DELETE_MANY)), Times.Once);
    }

    [Fact]
    public void Should_DeleteAll_When_Called()
    {
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(10);

        var result = _mockSession.Object.DeleteAll<AllPrimitive>();

        result.Should().Be(10);
        _mockSession.Verify(s => s.Execute(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.DELETE_ALL)), Times.Once);
    }

    #endregion

    #region Update

    [Fact]
    public void Should_Update_When_EntityIsValid()
    {
        var entity = new AllPrimitive { Id = 1, Boolean = true, Int64 = 100 };
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(1);

        var result = _mockSession.Object.Update(entity);

        result.Should().Be(1);
        _mockSession.Verify(s => s.Execute(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.UPDATE)), Times.Once);
    }

    [Fact]
    public void Should_UpdateWithTrack_When_EnablePropertyChangedTrackIsTrue()
    {
        var entity = new AllPrimitive { Id = 1, Boolean = true };
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(1);

        var result = _mockSession.Object.Update(entity, true);

        result.Should().Be(1);
    }

    [Fact]
    public void Should_DyUpdate_When_EntityIsValid()
    {
        var entity = new AllPrimitive { Id = 1, Boolean = true };
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(1);

        var result = _mockSession.Object.DyUpdate<AllPrimitive>(entity);

        result.Should().Be(1);
        _mockSession.Verify(s => s.Execute(It.Is<AbstractRequestContext>(ctx =>
            ctx.SqlId == CUDStatementName.UPDATE)), Times.Once);
    }

    [Fact]
    public void Should_DyUpdateWithTrack_When_EnablePropertyChangedTrackIsSet()
    {
        var entity = new AllPrimitive { Id = 1, Boolean = true };
        _mockSession.Setup(s => s.Execute(It.IsAny<AbstractRequestContext>())).Returns(1);

        var result = _mockSession.Object.DyUpdate<AllPrimitive>(entity, true);

        result.Should().Be(1);
    }

    #endregion
}
