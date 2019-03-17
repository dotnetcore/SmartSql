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
            //var pipeline = new PipelineBuilder()
            //       .Add(new InitializerMiddleware())
            //       .Add(new PrepareStatementMiddleware())
            //       .Add(new CachingMiddleware())
            //       .Add(new CommandExecuterMiddleware(new CommandExecuter()))
            //       .Add(new ResultHandlerMiddleware())
            //       .Build();
        }
    }
}
