using System.Linq;
using FluentAssertions;
using SmartSql.Middlewares;
using Xunit;

namespace SmartSql.Test.Unit.Middlewares;

public class PipelineOrderTests
{
    [Fact]
    public void Should_InitializerMiddlewareHaveOrderZero()
    {
        new InitializerMiddleware().Order.Should().Be(0);
    }

    [Fact]
    public void Should_PrepareStatementMiddlewareHaveOrderOneHundred()
    {
        new PrepareStatementMiddleware().Order.Should().Be(100);
    }

    [Fact]
    public void Should_CachingMiddlewareHaveOrderTwoHundred()
    {
        new CachingMiddleware().Order.Should().Be(200);
    }

    [Fact]
    public void Should_TransactionMiddlewareHaveOrderThreeHundred()
    {
        new TransactionMiddleware().Order.Should().Be(300);
    }

    [Fact]
    public void Should_DataSourceFilterMiddlewareHaveOrderFourHundred()
    {
        new DataSourceFilterMiddleware().Order.Should().Be(400);
    }

    [Fact]
    public void Should_CommandExecuterMiddlewareHaveOrderFiveHundred()
    {
        new CommandExecuterMiddleware().Order.Should().Be(500);
    }

    [Fact]
    public void Should_ResultHandlerMiddlewareHaveOrderSixHundred()
    {
        new ResultHandlerMiddleware().Order.Should().Be(600);
    }

    [Fact]
    public void Should_BuildPipelineInCorrectOrder()
    {
        var pipeline = new PipelineBuilder()
            .Add(new ResultHandlerMiddleware())
            .Add(new PrepareStatementMiddleware())
            .Add(new InitializerMiddleware())
            .Add(new CachingMiddleware())
            .Add(new CommandExecuterMiddleware())
            .Add(new DataSourceFilterMiddleware())
            .Add(new TransactionMiddleware())
            .Build();

        pipeline.Should().BeOfType<InitializerMiddleware>();

        IMiddleware middleware = pipeline.Next;
        middleware.Should().BeOfType<PrepareStatementMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<CachingMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<TransactionMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<DataSourceFilterMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<CommandExecuterMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<ResultHandlerMiddleware>();

        middleware.Next.Should().BeNull();
    }

    [Fact]
    public void Should_BuildPipelineInCorrectOrder_When_OnlyCoreMiddlewares()
    {
        var pipeline = new PipelineBuilder()
            .Add(new ResultHandlerMiddleware())
            .Add(new PrepareStatementMiddleware())
            .Add(new InitializerMiddleware())
            .Add(new CommandExecuterMiddleware())
            .Add(new DataSourceFilterMiddleware())
            .Add(new TransactionMiddleware())
            .Build();

        pipeline.Should().BeOfType<InitializerMiddleware>();

        IMiddleware middleware = pipeline.Next;
        middleware.Should().BeOfType<PrepareStatementMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<TransactionMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<DataSourceFilterMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<CommandExecuterMiddleware>();

        middleware = middleware.Next;
        middleware.Should().BeOfType<ResultHandlerMiddleware>();

        middleware.Next.Should().BeNull();
    }

    [Fact]
    public void Should_AllMiddlewareOrdersBeUnique()
    {
        var middlewares = new IMiddleware[]
        {
            new InitializerMiddleware(),
            new PrepareStatementMiddleware(),
            new CachingMiddleware(),
            new TransactionMiddleware(),
            new DataSourceFilterMiddleware(),
            new CommandExecuterMiddleware(),
            new ResultHandlerMiddleware()
        };

        var orders = middlewares.Select(m => m.Order).ToList();
        orders.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void Should_AllMiddlewareOrdersBeStrictlyIncreasing()
    {
        var middlewares = new IMiddleware[]
        {
            new InitializerMiddleware(),
            new PrepareStatementMiddleware(),
            new CachingMiddleware(),
            new TransactionMiddleware(),
            new DataSourceFilterMiddleware(),
            new CommandExecuterMiddleware(),
            new ResultHandlerMiddleware()
        };

        var sorted = middlewares.OrderBy(m => m.Order).ToList();
        sorted[0].Should().BeOfType<InitializerMiddleware>();
        sorted[1].Should().BeOfType<PrepareStatementMiddleware>();
        sorted[2].Should().BeOfType<CachingMiddleware>();
        sorted[3].Should().BeOfType<TransactionMiddleware>();
        sorted[4].Should().BeOfType<DataSourceFilterMiddleware>();
        sorted[5].Should().BeOfType<CommandExecuterMiddleware>();
        sorted[6].Should().BeOfType<ResultHandlerMiddleware>();
    }

    [Fact]
    public void Should_InitializerMiddlewareBeFirstInOrder()
    {
        var allOrders = new[]
        {
            new InitializerMiddleware().Order,
            new PrepareStatementMiddleware().Order,
            new CachingMiddleware().Order,
            new TransactionMiddleware().Order,
            new DataSourceFilterMiddleware().Order,
            new CommandExecuterMiddleware().Order,
            new ResultHandlerMiddleware().Order
        };

        allOrders.Min().Should().Be(0);
    }

    [Fact]
    public void Should_ResultHandlerMiddlewareBeLastInOrder()
    {
        var allOrders = new[]
        {
            new InitializerMiddleware().Order,
            new PrepareStatementMiddleware().Order,
            new CachingMiddleware().Order,
            new TransactionMiddleware().Order,
            new DataSourceFilterMiddleware().Order,
            new CommandExecuterMiddleware().Order,
            new ResultHandlerMiddleware().Order
        };

        allOrders.Max().Should().Be(600);
    }
}
