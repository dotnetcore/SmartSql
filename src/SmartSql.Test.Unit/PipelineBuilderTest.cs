using SmartSql.Command;
using SmartSql.Middlewares;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class PipelineBuilderTest
    {
        [Fact]
        public void Build()
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
            Assert.Equal(typeof(InitializerMiddleware), pipeline.GetType());
            IMiddleware middleware = pipeline.Next;
            Assert.Equal(typeof(PrepareStatementMiddleware), middleware.GetType());
            middleware = middleware.Next;
            Assert.Equal(typeof(CachingMiddleware), middleware.GetType());
            middleware = middleware.Next;
            Assert.Equal(typeof(TransactionMiddleware), middleware.GetType());
            middleware = middleware.Next;
            Assert.Equal(typeof(DataSourceFilterMiddleware), middleware.GetType());
            middleware = middleware.Next;
            Assert.Equal(typeof(CommandExecuterMiddleware), middleware.GetType());
            middleware = middleware.Next;
            Assert.Equal(typeof(ResultHandlerMiddleware), middleware.GetType());
        }
    }
}
