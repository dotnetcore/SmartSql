using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit.CUD;

public class ISqlMapperExtensionsTests
{
    private readonly Mock<ISqlMapper> _mockMapper;
    private readonly Mock<IDbSessionStore> _mockSessionStore;
    private readonly Mock<IDbSession> _mockSession;

    public ISqlMapperExtensionsTests()
    {
        _mockMapper = new Mock<ISqlMapper>();
        _mockSessionStore = new Mock<IDbSessionStore>();
        _mockSession = new Mock<IDbSession>();

        _mockMapper.Setup(m => m.SessionStore).Returns(_mockSessionStore.Object);
        _mockSessionStore.Setup(s => s.Open()).Returns(_mockSession.Object);
    }

    #region Session Lifecycle

    [Fact]
    public void Should_OpenNewSession_When_NoLocalSessionExists()
    {
        // Arrange
        _mockSessionStore.Setup(s => s.LocalSession).Returns((IDbSession)null);
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>()))
            .Returns(new List<dynamic>());
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        _mockMapper.Object.QueryDynamic(requestContext);

        // Assert
        _mockSessionStore.Verify(s => s.Open(), Times.Once);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public void Should_ReuseLocalSession_When_LocalSessionExists()
    {
        // Arrange
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>()))
            .Returns(new List<dynamic>());
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        _mockMapper.Object.QueryDynamic(requestContext);

        // Assert
        _mockSessionStore.Verify(s => s.Open(), Times.Never);
        _mockSessionStore.Verify(s => s.Dispose(), Times.Never);
    }

    [Fact]
    public void Should_DisposeSession_When_OwnSessionAndExceptionThrown()
    {
        // Arrange
        _mockSessionStore.Setup(s => s.LocalSession).Returns((IDbSession)null);
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>()))
            .Throws<InvalidOperationException>();
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        Action act = () => _mockMapper.Object.QueryDynamic(requestContext);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _mockSessionStore.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public void Should_NotDisposeSession_When_UsingLocalSessionAndExceptionThrown()
    {
        // Arrange
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.Query<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Throws<InvalidOperationException>();
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        Action act = () => _mockMapper.Object.QueryDictionary(requestContext);

        // Assert
        act.Should().Throw<InvalidOperationException>();
        _mockSessionStore.Verify(s => s.Dispose(), Times.Never);
    }

    #endregion

    #region QueryDynamic

    [Fact]
    public void Should_QueryDynamic_When_QueryDynamicCalled()
    {
        // Arrange
        var expected = new List<dynamic> { new { Id = 1 } };
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>())).Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        var result = _mockMapper.Object.QueryDynamic(requestContext);

        // Assert
        result.Should().BeSameAs(expected);
    }

    [Fact]
    public void Should_ReturnEmptyList_When_QueryDynamicReturnsNoResults()
    {
        // Arrange
        var empty = new List<dynamic>();
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.Query<dynamic>(It.IsAny<AbstractRequestContext>())).Returns(empty);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QueryEmpty" };

        // Act
        var result = _mockMapper.Object.QueryDynamic(requestContext);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region QueryDictionary

    [Fact]
    public void Should_QueryDictionary_When_QueryDictionaryCalled()
    {
        // Arrange
        var expected = new List<IDictionary<string, object>>
        {
            new Dictionary<string, object> { { "Id", 1 } }
        };
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.Query<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "Query" };

        // Act
        var result = _mockMapper.Object.QueryDictionary(requestContext);

        // Assert
        result.Should().BeSameAs(expected);
    }

    [Fact]
    public void Should_ReturnEmptyDictionaryList_When_QueryDictionaryReturnsNoResults()
    {
        // Arrange
        var empty = new List<IDictionary<string, object>>();
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.Query<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns(empty);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QueryEmpty" };

        // Act
        var result = _mockMapper.Object.QueryDictionary(requestContext);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region QuerySingleDynamic

    [Fact]
    public void Should_QuerySingleDynamic_When_QuerySingleDynamicCalled()
    {
        // Arrange
        object expected = new { Id = 1 };
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.QuerySingle<object>(It.IsAny<AbstractRequestContext>()))
            .Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        object result = _mockMapper.Object.QuerySingleDynamic(requestContext);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Should_ReturnNull_When_QuerySingleDynamicReturnsNull()
    {
        // Arrange
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.QuerySingle<object>(It.IsAny<AbstractRequestContext>()))
            .Returns((object)null);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        object result = _mockMapper.Object.QuerySingleDynamic(requestContext);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region QuerySingleDictionary

    [Fact]
    public void Should_QuerySingleDictionary_When_QuerySingleDictionaryCalled()
    {
        // Arrange
        var expected = new Dictionary<string, object> { { "Id", 1 } };
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.QuerySingle<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns(expected);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        var result = _mockMapper.Object.QuerySingleDictionary(requestContext);

        // Assert
        result.Should().BeSameAs(expected);
    }

    [Fact]
    public void Should_ReturnNull_When_QuerySingleDictionaryReturnsNull()
    {
        // Arrange
        _mockSessionStore.Setup(s => s.LocalSession).Returns(_mockSession.Object);
        _mockSession.Setup(s => s.QuerySingle<IDictionary<string, object>>(It.IsAny<AbstractRequestContext>()))
            .Returns((IDictionary<string, object>)null);
        var requestContext = new RequestContext { Scope = "Test", SqlId = "QuerySingle" };

        // Act
        var result = _mockMapper.Object.QuerySingleDictionary(requestContext);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}
