using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Middlewares
{
    public class TransactionMiddleware : IMiddleware
    {
        public IMiddleware Next { get; set; }
        public void Invoke<TResult>(ExecutionContext executionContext)
        {
            var isTransaction = executionContext.DbSession.Transaction != null;
            if (isTransaction || !executionContext.Request.Transaction.HasValue)
            {
                Next.Invoke<TResult>(executionContext);
            }
            else
            {
                executionContext.DbSession.TransactionWrap(executionContext.Request.Transaction.Value,
                    () => { Next.Invoke<TResult>(executionContext); });
            }
        }

        public async Task InvokeAsync<TResult>(ExecutionContext executionContext)
        {
            var isTransaction = executionContext.DbSession.Transaction != null;
            if (isTransaction || !executionContext.Request.Transaction.HasValue)
            {
                await Next.InvokeAsync<TResult>(executionContext);
            }
            else
            {
                await executionContext.DbSession.TransactionWrapAsync(executionContext.Request.Transaction.Value,
                      async () => { await Next.InvokeAsync<TResult>(executionContext); });
            }
        }
    }
}
