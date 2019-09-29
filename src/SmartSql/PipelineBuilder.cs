using SmartSql.Command;
using SmartSql.Configuration;
using SmartSql.Middlewares;
using System.Collections.Generic;
using System.Linq;

namespace SmartSql
{
    public class PipelineBuilder
    {
        public IList<IMiddleware> Pipeline { get; private set; }

        public PipelineBuilder()
        {
            Pipeline = new List<IMiddleware>();
        }

        public PipelineBuilder Add(IMiddleware middleware)
        {
            Pipeline.Add(middleware);
            return this;
        }

        public IMiddleware Build()
        {
            var list = Pipeline.OrderBy(middleware => middleware.Order).ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var current = Pipeline[i];
                if (i == Pipeline.Count - 1)
                {
                    break;
                }

                current.Next = Pipeline[i + 1];
            }

            return Pipeline.FirstOrDefault();
        }
    }
}