using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SmartSql.Exceptions;

namespace SmartSql.AOP
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TransactionAttribute : AbstractInterceptorAttribute
    {
        public string Alias { get; set; } = SmartSqlBuilder.DEFAULT_ALIAS;
        public IsolationLevel Level { get; set; } = IsolationLevel.Unspecified;
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var sessionStore = context.ServiceProvider.GetSessionStore(Alias);
            if (sessionStore == null)
            {
                throw new SmartSqlException($"can not find SmartSql instance by Alias:{Alias}.");
            }
            var inTransaction = sessionStore.LocalSession?.Transaction != null;
            if (inTransaction)
            {
                await next.Invoke(context); return;
            }

            using (sessionStore)
            {
                await sessionStore.Open().TransactionWrapAsync(Level, async () =>
                {
                    await next.Invoke(context);
                });
            }
        }
    }
}
