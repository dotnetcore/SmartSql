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
                var current = list[i];
                if (i == list.Count - 1)
                {
                    break;
                }

                current.Next = list[i + 1];
            }

            return list.FirstOrDefault();
        }
    }
}