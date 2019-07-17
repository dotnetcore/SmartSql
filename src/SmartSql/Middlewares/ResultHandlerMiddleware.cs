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
        private IDeserializerFactory _deserializerFactory;

        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            var resultContext = executionContext.Result;
            var deser = _deserializerFactory.Get(executionContext, typeof(TResult),
                executionContext.Request.MultipleResultMap != null);
            try
            {
                if (resultContext.IsList)
                {
                    resultContext.SetData(deser.ToList<TResult>(executionContext));
                }
                else
                {
                    resultContext.SetData(deser.ToSinge<TResult>(executionContext));
                }
            }
            finally
            {
                if (executionContext.DataReaderWrapper != null)
                {
                    executionContext.DataReaderWrapper.Close();
                    executionContext.DataReaderWrapper.Dispose();
                }
            }

            InvokeNext<TResult>(executionContext);
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            var resultContext = executionContext.Result;
            var deser = _deserializerFactory.Get(executionContext, typeof(TResult));
            try
            {
                if (resultContext.IsList)
                {
                    resultContext.SetData(await deser.ToListAsync<TResult>(executionContext));
                }
                else
                {
                    resultContext.SetData(await deser.ToSingeAsync<TResult>(executionContext));
                }
            }
            finally
            {
                if (executionContext.DataReaderWrapper != null)
                {
                    executionContext.DataReaderWrapper.Close();
                    executionContext.DataReaderWrapper.Dispose();
                }
            }

            await InvokeNextAsync<TResult>(executionContext);
        }

        public override void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _deserializerFactory = smartSqlBuilder.SmartSqlConfig.DeserializerFactory;
        }
    }
}