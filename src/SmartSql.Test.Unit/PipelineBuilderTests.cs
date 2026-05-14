using FluentAssertions;
using SmartSql.Middlewares;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class PipelineBuilderTests
    {
        [Fact]
        public void Should_BuildOrderedPipeline_When_AllMiddlewareAdded()
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
        }
    }
}
