using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit.DbSession;

public class DbSessionExtensionsTests
{
    private readonly Mock<IDbSession> _mockSession;

    public DbSessionExtensionsTests()
    {
        _mockSession = new Mock<IDbSession>();
    }

    #region QueryDynamic

    [Fact]
    public void Should_ReturnDynamicList_When_QueryDynamicCalled()
    {
        // Arrange
        var expected = new List<dynamic> { new { Id = 1 }, new { Id = 2 } };
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>())).Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        var result = _mockSession.Object.QueryDynamic(requestContext);

        // Assert
        result.Should().BeSameAs(expected);
        _mockSession.Verify(s => s.Query<dynamic>(requestContext), Times.Once);
    }

    [Fact]
    public void Should_ReturnEmptyList_When_QueryDynamicReturnsNoResults()
    {
        // Arrange
        var empty = new List<dynamic>();
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>())).Returns(empty);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QueryEmpty" };

        // Act
        var result = _mockSession.Object.QueryDynamic(requestContext);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region QueryDictionary

    [Fact]
    public void Should_ReturnDictionaryList_When_QueryDictionaryCalled()
    {
        // Arrange
        var expected = new List<IDictionary<string, object>>
        {
            new Dictionary<string, object> { { "Id", 1 }, { "Name", "Test" } }
        };
        _mockSession.Setup(s => s.Query<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        var result = _mockSession.Object.QueryDictionary(requestContext);

        // Assert
        result.Should().BeSameAs(expected);
        _mockSession.Verify(s => s.Query<IDictionary<string, object>>(requestContext), Times.Once);
    }

    [Fact]
    public void Should_ReturnEmptyDictionaryList_When_QueryDictionaryReturnsNoResults()
    {
        // Arrange
        var empty = new List<IDictionary<string, object>>();
        _mockSession.Setup(s => s.Query<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns(empty);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QueryEmpty" };

        // Act
        var result = _mockSession.Object.QueryDictionary(requestContext);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region QuerySingleDynamic

    [Fact]
    public void Should_ReturnDynamicSingle_When_QuerySingleDynamicCalled()
    {
        // Arrange
        object expected = new { Id = 1, Name = "Test" };
        _mockSession.Setup(s => s.QuerySingle<object>(It.IsAny<AbstractRequestContext>()))
            .Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        object result = _mockSession.Object.QuerySingleDynamic(requestContext);

        // Assert
        result.Should().NotBeNull();
        _mockSession.Verify(s => s.QuerySingle<object>(requestContext), Times.Once);
    }

    [Fact]
    public void Should_ReturnNullDynamic_When_QuerySingleDynamicReturnsNull()
    {
        // Arrange
        _mockSession.Setup(s => s.QuerySingle<object>(It.IsAny<AbstractRequestContext>()))
            .Returns((object)null);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        object result = _mockSession.Object.QuerySingleDynamic(requestContext);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region QuerySingleDictionary

    [Fact]
    public void Should_ReturnDictionarySingle_When_QuerySingleDictionaryCalled()
    {
        // Arrange
        var expected = new Dictionary<string, object> { { "Id", 1 }, { "Name", "Test" } };
        _mockSession.Setup(s => s.QuerySingle<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        var result = _mockSession.Object.QuerySingleDictionary(requestContext);

        // Assert
        result.Should().BeSameAs(expected);
        _mockSession.Verify(s => s.QuerySingle<IDictionary<string, object>>(requestContext), Times.Once);
    }

    [Fact]
    public void Should_ReturnNullDictionary_When_QuerySingleDictionaryReturnsNull()
    {
        // Arrange
        _mockSession.Setup(s => s.QuerySingle<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns((IDictionary<string, object>)null);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        var result = _mockSession.Object.QuerySingleDictionary(requestContext);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region QueryDynamic with null results

    [Fact]
    public void Should_HandleNullList_When_QueryDynamicReturnsNull()
    {
        // Arrange
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>()))
            .Returns((IList<dynamic>)null);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QueryNull" };

        // Act
        var result = _mockSession.Object.QueryDynamic(requestContext);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
