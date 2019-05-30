using SmartSql.Configuration;
using SmartSql.Deserializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class ResultHandlerMiddleware : AbstractMiddleware
    {
        private readonly IDeserializerFactory _deserializerFactory;

        public ResultHandlerMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _deserializerFactory = smartSqlConfig.DeserializerFactory;
        }

        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            var resultContext = executionContext.Result;
            var deser = _deserializerFactory.Get(executionContext, typeof(TResult),
                executionContext.Request.MultipleResultMap != null);
            if (resultContext.IsList)
            {
                resultContext.SetData(deser.ToList<TResult>(executionContext));
            }
            else
            {
                resultContext.SetData(deser.ToSinge<TResult>(executionContext));
            }

            InvokeNext<TResult>(executionContext);
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            var resultContext = executionContext.Result;
            var deser = _deserializerFactory.Get(executionContext, typeof(TResult));
            if (resultContext.IsList)
            {
                resultContext.SetData(await deser.ToListAsync<TResult>(executionContext));
            }
            else
            {
                resultContext.SetData(await deser.ToSingeAsync<TResult>(executionContext));
            }

            if (Next != null)
            {
                await InvokeNextAsync<TResult>(executionContext);
            }
        }
    }
}