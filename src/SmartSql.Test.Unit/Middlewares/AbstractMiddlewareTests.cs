using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Middlewares;
using Xunit;

namespace SmartSql.Test.Unit.Middlewares;

public class AbstractMiddlewareTests
{
    private class TestMiddleware : AbstractMiddleware
    {
        public bool InvokeNextCalled { get; private set; }
        public bool SelfInvokeCalled { get; private set; }
        public bool OnNextInvokedCalled { get; private set; }
        public List<ExecutionContext> InvokingContexts { get; } = new List<ExecutionContext>();
        public List<ExecutionContext> NextInvokedContexts { get; } = new List<ExecutionContext>();

        public override int Order => 100;

        protected override void SelfInvoke<TResult>(ExecutionContext executionContext)
        {
            SelfInvokeCalled = true;
        }

        protected override void OnNextInvoked<TResult>(ExecutionContext executionContext)
        {
            OnNextInvokedCalled = true;
            NextInvokedContexts.Add(executionContext);
        }
    }

    private class FilterCaptureMiddleware : AbstractMiddleware
    {
        public List<ExecutionContext> OnInvokingCalls { get; } = new List<ExecutionContext>();
        public List<ExecutionContext> OnInvokedCalls { get; } = new List<ExecutionContext>();

        public override int Order => 50;

        private void TrackFilters()
        {
            var filtersField = typeof(AbstractMiddleware).GetProperty("Filters",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var filters = filtersField.GetValue(this) as IList<SmartSql.Middlewares.Filters.IInvokeMiddlewareFilter>;

            if (filters == null) return;
            foreach (var filter in filters)
            {
                filter.OnInvoking(null);
                filter.OnInvoked(null);
            }
        }

        public void AssertFiltersCalled()
        {
            TrackFilters();
        }
    }

    private static ExecutionContext CreateContext(bool endResult = false)
    {
        return new ExecutionContext
        {
            Request = new RequestContext<object>(),
            Result = new SingleResultContext<string> { End = endResult },
            DbSession = new Mock<IDbSession>().Object,
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
    public void Should_HaveCorrectOrder_When_Created()
    {
        var middleware = new TestMiddleware();

        middleware.Order.Should().Be(100);
    }

    [Fact]
    public void Should_CallSelfInvoke_When_InvokeCalled()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();

        middleware.Invoke<string>(context);

        middleware.SelfInvokeCalled.Should().BeTrue();
    }

    [Fact]
    public void Should_CallInvokeNext_When_NextExists()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<object>(context)).Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        middleware.Invoke<object>(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public void Should_NotThrow_When_NextIsNull()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        middleware.Next = null;

        var act = () => middleware.Invoke<string>(context);

        act.Should().NotThrow();
    }

    [Fact]
    public void Should_CallOnNextInvoked_When_NextInvoked()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        var mockNext = new Mock<IMiddleware>();
        middleware.Next = mockNext.Object;

        middleware.Invoke<object>(context);

        middleware.OnNextInvokedCalled.Should().BeTrue();
        middleware.NextInvokedContexts.Should().ContainSingle();
    }

    [Fact]
    public void Should_NotInvokeNext_When_ResultIsEnded()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext(endResult: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<object>(context)).Callback(() => nextCalled = true);
        middleware.Next = mockNext.Object;

        middleware.Invoke<object>(context);

        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_NotInvokeNextAsync_When_ResultIsEnded()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext(endResult: true);
        var nextCalled = false;

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<object>(context)).Callback(() => nextCalled = true).Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        await middleware.InvokeAsync<object>(context);

        nextCalled.Should().BeFalse();
    }

    [Fact]
    public void Should_PropagateException_When_NextThrows()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        var expectedException = new InvalidOperationException("Test exception");

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.Invoke<object>(context)).Throws(expectedException);
        middleware.Next = mockNext.Object;

        var act = () => middleware.Invoke<object>(context);

        act.Should().Throw<InvalidOperationException>().WithMessage("Test exception");
    }

    [Fact]
    public async Task Should_PropagateExceptionAsync_When_NextThrowsAsync()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        var expectedException = new InvalidOperationException("Async test exception");

        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<object>(context)).ThrowsAsync(expectedException);
        middleware.Next = mockNext.Object;

        var act = async () => await middleware.InvokeAsync<object>(context);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Async test exception");
    }

    [Fact]
    public void Should_CallSelfInvoke_When_CustomSelfInvokeOverride()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        var mockNext = new Mock<IMiddleware>();
        middleware.Next = mockNext.Object;

        middleware.Invoke<string>(context);

        middleware.SelfInvokeCalled.Should().BeTrue();
    }

    [Fact]
    public void Should_SkipNext_When_NextIsNull()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        middleware.Next = null;

        var act = () => middleware.Invoke<object>(context);

        act.Should().NotThrow();
    }

    [Fact]
    public async Task Should_BeCalledInvokeAsync_When_NextNotEnded()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext(endResult: false);
        var nextCalled = false;
        var mockNext = new Mock<IMiddleware>();
        mockNext.Setup(m => m.InvokeAsync<object>(context)).Callback(() => nextCalled = true).Returns(Task.CompletedTask);
        middleware.Next = mockNext.Object;

        await middleware.InvokeAsync<object>(context);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CompleteSuccessfully_When_InvokeAsyncWithNoNext()
    {
        var middleware = new TestMiddleware();
        var context = CreateContext();
        middleware.Next = null;

        var act = async () => await middleware.InvokeAsync<object>(context);

        await act.Should().CompleteWithinAsync(TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Should_SetNext_When_NextAssigned()
    {
        var middleware = new TestMiddleware();
        var mockNext = new Mock<IMiddleware>();

        middleware.Next = mockNext.Object;

        middleware.Next.Should().BeSameAs(mockNext.Object);
    }
}
