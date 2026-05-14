using System;
using FluentAssertions;
using Moq;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit.DbSession;

public class DbSessionStoreTests
{
    [Fact]
    public void Should_ReturnNullLocalSession_When_NoSessionOpened()
    {
        var factory = new Mock<IDbSessionFactory>();
        var store = new DbSessionStore(factory.Object);

        store.LocalSession.Should().BeNull();
    }

    [Fact]
    public void Should_CreateSession_When_OpenCalled()
    {
        var mockSession = new Mock<IDbSession>();
        var factory = new Mock<IDbSessionFactory>();
        factory.Setup(f => f.Open()).Returns(mockSession.Object);
        var store = new DbSessionStore(factory.Object);

        var session = store.Open();

        session.Should().NotBeNull();
        session.Should().BeSameAs(mockSession.Object);
    }

    [Fact]
    public void Should_ReturnSameSession_When_OpenCalledTwice()
    {
        var mockSession = new Mock<IDbSession>();
        var factory = new Mock<IDbSessionFactory>();
        factory.Setup(f => f.Open()).Returns(mockSession.Object);
        var store = new DbSessionStore(factory.Object);

        var first = store.Open();
        var second = store.Open();

        first.Should().BeSameAs(second);
        factory.Verify(f => f.Open(), Times.Once);
    }

    [Fact]
    public void Should_DisposeSession_When_DisposeCalled()
    {
        var mockSession = new Mock<IDbSession>();
        var factory = new Mock<IDbSessionFactory>();
        factory.Setup(f => f.Open()).Returns(mockSession.Object);
        var store = new DbSessionStore(factory.Object);

        store.Open();
        store.Dispose();

        store.LocalSession.Should().BeNull();
        mockSession.Verify(s => s.Dispose(), Times.Once);
    }

    [Fact]
    public void Should_NotThrow_When_DisposeCalledWithoutOpening()
    {
        var factory = new Mock<IDbSessionFactory>();
        var store = new DbSessionStore(factory.Object);

        Action act = () => store.Dispose();

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_OpenNewSession_When_OpenCalledAfterDispose()
    {
        var mockSession1 = new Mock<IDbSession>();
        var mockSession2 = new Mock<IDbSession>();
        var factory = new Mock<IDbSessionFactory>();
        factory.SetupSequence(f => f.Open())
            .Returns(mockSession1.Object)
            .Returns(mockSession2.Object);
        var store = new DbSessionStore(factory.Object);

        store.Open();
        store.Dispose();
        var newSession = store.Open();

        newSession.Should().BeSameAs(mockSession2.Object);
    }
}
