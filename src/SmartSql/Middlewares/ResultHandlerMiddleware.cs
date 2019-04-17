
using SmartSql.Configuration;
using SmartSql.Deserializer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class ResultHandlerMiddleware : IMiddleware
    {
        private readonly IDeserializerFactory _deserializerFactory;
        public IMiddleware Next { get; set; }
        public ResultHandlerMiddleware(SmartSqlConfig smartSqlConfig)
        {
            _deserializerFactory = smartSqlConfig.DeserializerFactory;
        }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            var resultContext = executionContext.Result;
            var deser = _deserializerFactory.Get(executionContext, typeof(TResult));
            if (resultContext.IsList)
            {
                resultContext.SetData(deser.ToList<TResult>(executionContext));
            }
            else
            {
                resultContext.SetData(deser.ToSinge<TResult>(executionContext));
            }
        }

        public async Task InvokeAsync<TResult>(ExecutionContext executionContext)
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
        }
    }
}
