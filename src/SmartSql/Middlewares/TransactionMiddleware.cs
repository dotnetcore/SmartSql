using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class TransactionMiddleware : AbstractMiddleware
    {
        public override void Invoke<TResult>(ExecutionContext executionContext)
        {
            var isTransaction = executionContext.DbSession.Transaction != null;
            if (isTransaction || !executionContext.Request.Transaction.HasValue)
            {
                InvokeNext<TResult>(executionContext);
            }
            else
            {
                executionContext.DbSession.TransactionWrap(executionContext.Request.Transaction.Value,
                    () => { InvokeNext<TResult>(executionContext); });
            }
        }

        public override async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            var isTransaction = executionContext.DbSession.Transaction != null;
            if (isTransaction || !executionContext.Request.Transaction.HasValue)
            {
                await InvokeNextAsync<TResult>(executionContext);
            }
            else
            {
                await executionContext.DbSession.TransactionWrapAsync(executionContext.Request.Transaction.Value,
                      async () => { await InvokeNextAsync<TResult>(executionContext); });
            }
        }

        public override void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            
        }

        public override int Order => 300;
    }
}
